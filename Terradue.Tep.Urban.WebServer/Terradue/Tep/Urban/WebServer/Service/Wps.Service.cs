using Terradue.Tep.WebServer.Services;
using ServiceStack.ServiceHost;
using ServiceStack.Common.Web;
using Terradue.Tep.WebServer;
using Terradue.Portal;
using System;
using System.Net;
using OpenGis.Wps;
using System.Collections.Generic;
using System.IO;
using ServiceStack.Text;
using System.Runtime.Serialization;
using System.Linq;
using Terradue.WebService.Model;
using System.Web;

namespace Terradue.Tep.Urban.WebServer.Services
{
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------

    [Route("/utep/wps/WebProcessingService", "POST", Summary = "POST the wps", Notes = "")]
    public class WpsExecutePostRequestTepUrban : IRequiresRequestStream, IReturn<HttpResult> {
        public System.IO.Stream RequestStream { get; set; }
    }

    [Route("/utep/wps/WebProcessingService", "GET", Summary = "Web Processing Services", Notes = "")]
    public class WpsRequestTepUrban : IReturn<HttpResult>{
        [ApiMember(Name="service", Description = "service type", ParameterType = "query", DataType = "String", IsRequired = true)]
        public string Service { get; set; }
        [ApiMember(Name="request", Description = "request type", ParameterType = "query", DataType = "String", IsRequired = true)]
        public string Request { get; set; }
        [ApiMember(Name="identifier", Description = "request identifier", ParameterType = "query", DataType = "String", IsRequired = true)]
        public string Identifier { get; set; }
        [ApiMember(Name="version", Description = "request version", ParameterType = "query", DataType = "String", IsRequired = true)]
        public string Version { get; set; }
        [ApiMember(Name="dataInputs", Description = "request data inputs", ParameterType = "query", DataType = "String", IsRequired = true)]
        public string DataInputs { get; set; }
        [ApiMember(Name="responseDocument", Description = "request response document", ParameterType = "query", DataType = "String", IsRequired = true)]
        public string ResponseDocument { get; set; }
    }

    [Api("Tep Urban Terradue webserver")]
    [Restrict(EndpointAttributes.InSecure | EndpointAttributes.InternalNetworkAccess | EndpointAttributes.Json,
              EndpointAttributes.Secure   | EndpointAttributes.External | EndpointAttributes.Json)]
    public class WpsServiceTepUrban : WpsServiceTep {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public object Get(WpsRequestTepUrban request) {
            IfyWebContext context;
            System.IO.Stream stream = new System.IO.MemoryStream();
            context = TepWebContext.GetWebContext(PagePrivileges.EverybodyView);
            context.RestrictedMode = false;

            context.Open();

            if (request.Service.ToLower() != "wps")
                throw new Exception("Web Processing Service Request is not valid");
            WpsProcessOffering wps = null;
            switch (request.Request.ToLower()) {
                case "getcapabilities":
                    log.Info("WPS GetCapabilities requested");
                    try{
                        WPSCapabilitiesType getCapabilities = WpsFactory.CreateGetCapabilititesTemplate(context.BaseUrl + "/utep/wps/WebProcessingService");
                        getCapabilities.ServiceProvider.ProviderName = "Urban Tep";
                        getCapabilities.ServiceProvider.ProviderSite = new OnlineResourceType{ href = "https://urban-tep.eo.esa.int/" };

                        getCapabilities.ProcessOfferings.Process.Add(
                            new ProcessBriefType{
                                Identifier = new CodeType{ Value = "PUMA" },
                                Title = new LanguageStringType{ Value = "Analyse in PUMA" },
                                Abstract = new LanguageStringType { Value = "This service will upload and start a run analysis on PUMA for further visualization in PUMA interface." },
                            }
                        );

                        context.Close();
                        System.Xml.Serialization.XmlSerializerNamespaces ns = new System.Xml.Serialization.XmlSerializerNamespaces();
                        ns.Add("wps", "http://www.opengis.net/wps/1.0.0");
                        ns.Add("ows", "http://www.opengis.net/ows/1.1");
                        ns.Add("xlink", "http://www.w3.org/1999/xlink");

                        new System.Xml.Serialization.XmlSerializer(typeof(WPSCapabilitiesType)).Serialize(stream, getCapabilities, ns);
                        return new HttpResult(stream, "application/xml");
                    }catch(Exception e){
                        return new HttpError(HttpStatusCode.BadRequest, e);
                    }

                case "describeprocess":
                    log.Info("WPS DescribeProcess requested");
                    try{
                        //PUMA process
                        var describeProcess = new ProcessDescriptionType();
                        describeProcess.Identifier = new CodeType{ Value = "PUMA" };
                        describeProcess.Title = new LanguageStringType{ Value = "Analyse in PUMA" };
                        describeProcess.Abstract = new LanguageStringType { Value = "This service will upload and start a run analysis on PUMA for further visualization in PUMA interface." };
                        describeProcess.DataInputs = new List<InputDescriptionType>();
                        describeProcess.DataInputs.Add(
                            new InputDescriptionType{
                                minOccurs = "1",
                                maxOccurs = "1",
                                Identifier = new CodeType{ Value = "url" },
                                Title = new LanguageStringType{ Value = "Url" },
                                Abstract = new LanguageStringType { Value = "Url of the Geotiff to be processed" },
                                LiteralData = new LiteralInputType{ DataType = new DomainMetadataType{ Value = "string" } }
                            }
                        );

                        var describeResponse = new ProcessDescriptions();
                        describeResponse.ProcessDescription = new List<ProcessDescriptionType>();
                        describeResponse.ProcessDescription.Add(describeProcess);

                        System.Xml.Serialization.XmlSerializerNamespaces ns = new System.Xml.Serialization.XmlSerializerNamespaces();
                        ns.Add("wps", "http://www.opengis.net/wps/1.0.0");
                        new System.Xml.Serialization.XmlSerializer(typeof(ProcessDescriptions)).Serialize(stream, describeResponse, ns);    
                        return new HttpResult(stream, "application/xml");

                    }catch(Exception e){
                        return new HttpError(HttpStatusCode.BadRequest, e.Message);
                    }

                case "execute":
                    log.Info("WPS Execute requested");
                    Execute executeInput = new Execute();

                    executeInput.Identifier = new CodeType{ Value = request.Identifier};
                    executeInput.service = request.Service;
                    executeInput.version = request.Version;
                    executeInput.DataInputs = new List<InputType>();
                    foreach (var param in request.DataInputs.Split(";".ToCharArray())) {
                        var key = param.Substring(0,param.IndexOf("="));
                        var value = param.Substring(param.IndexOf("=") + 1);
                        InputType input = new InputType();
                        input.Identifier = new CodeType{ Value = key };
                        input.Data = new DataType{ Item = new LiteralDataType{ Value = value } };
                        executeInput.DataInputs.Add(input);
                    }

                    var response = Execute(context, executeInput);
                    context.Close();
                    return response;
                default:
                    context.Close();
                    throw new Exception("Web Processing Service Request is not valid");
            }

        }

        public object Post(WpsExecutePostRequestTepUrban request) {
            IfyWebContext context = TepWebContext.GetWebContext(PagePrivileges.DeveloperView);
            context.Open();
            log.Info("WPS Execute requested (POST)");

            Execute executeInput = (Execute)new System.Xml.Serialization.XmlSerializer(typeof(Execute)).Deserialize(request.RequestStream);
            log.Debug("Deserialization done");
            var response = Execute(context, executeInput);
            context.Close();
            return response;
        }

        private object Execute(IfyContext context, Execute executeInput){
            WpsProcessOffering wps = CloudWpsFactory.GetWpsProcessOffering(context, executeInput.Identifier.Value);
            ExecuteResponse executeResponse = new ExecuteResponse();
            executeResponse.DataInputs = executeInput.DataInputs;

            try{
                if(executeInput.Identifier == null || executeInput.Identifier.Value == null) throw new Exception("Invalid identifier");
                switch(wps.RemoteIdentifier){
                    case "PUMA":
                        var gisatProcessUrl = context.GetConfigValue("gisat_processUrl");
                        NetworkCredential credentials = null;
                        var uri = new UriBuilder (gisatProcessUrl);
                        if (!string.IsNullOrEmpty (uri.UserName) && !string.IsNullOrEmpty (uri.Password)) credentials = new NetworkCredential (uri.UserName, uri.Password);
                        HttpWebRequest executeHttpRequest = WpsProvider.CreateWebRequest (gisatProcessUrl, credentials, context.Username);
                        executeHttpRequest.Method = "POST";
                        executeHttpRequest.ContentType = "application/json";
                        executeHttpRequest.Headers.Remove("REMOTE_USER");

                        Dictionary<string, string> inputparams = new Dictionary<string, string>();
                        foreach (var d in executeInput.DataInputs) {
                            log.Debug("Input: " + d.Identifier.Value);
                            if (d.Data != null && d.Data.Item != null) {
                                if (d.Data.Item is LiteralDataType) {
                                    log.Debug("Value is LiteralDataType");
                                    inputparams.Add(d.Identifier.Value, ((LiteralDataType)(d.Data.Item)).Value);  
                                } else if (d.Data.Item is ComplexDataType) {
                                    log.Debug("Value is ComplexDataType");
                                    throw new Exception("Data Input ComplexDataType not yet implemented");
                                } else if (d.Data.Item is BoundingBoxType) {
                                    //for BoundingBoxType, webportal creates LowerCorner and UpperCorner
                                    //we just need to save both values as a concatained string
                                    log.Debug("Value is BoundingBoxType");
                                    var bbox = d.Data.Item as BoundingBoxType;
                                    var bboxVal = (bbox != null && bbox.UpperCorner != null && bbox.LowerCorner != null) ? bbox.LowerCorner.Replace(" ", ",") + "," + bbox.UpperCorner.Replace(" ", ",") : "";
                                    inputparams.Add(d.Identifier.Value, bboxVal);  
                                } else {
                                    throw new Exception("unhandled type of Data");
                                } 
                            } else if (d.Reference != null){
                                log.Debug("Value is InputReferenceType");
                                if (!string.IsNullOrEmpty(d.Reference.href)) {
                                    inputparams.Add(d.Identifier.Value, d.Reference.href);
                                } else if (d.Reference.Item != null){
                                    throw new Exception("Data Input InputReferenceType item not yet implemented");
                                }
                            }
                        }

                        PumaExecuteRequest jsonrequest = new PumaExecuteRequest{
                            url = inputparams["url"]
                        };
                        string json = JsonSerializer.SerializeToString<PumaExecuteRequest>(jsonrequest);

                        using (var streamWriter = new StreamWriter(executeHttpRequest.GetRequestStream())) {
                            streamWriter.Write(json);
                            streamWriter.Flush();
                            streamWriter.Close();

                            using (var httpResponse = (HttpWebResponse)executeHttpRequest.GetResponse()){
                                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                                    string result = streamReader.ReadToEnd();
                                    try {
                                        PumaExecuteResponse response = JsonSerializer.DeserializeFromString<PumaExecuteResponse>(result);
                                        log.Debug("Execute response ok");
                                        log.Debug("identifier = " + response.id);
                                        string newId = Guid.NewGuid().ToString();
                                        WpsJob wpsjob = new WpsJob(context);
                                        wpsjob.Name = "Analyse in PUMA";
                                        wpsjob.RemoteIdentifier = response.id;
                                        wpsjob.Identifier = newId;
                                        wpsjob.OwnerId = context.UserId;
                                        wpsjob.UserId = context.UserId;
                                        wpsjob.WpsId = wps.Provider.Identifier;
                                        wpsjob.ProcessId = wps.Identifier;
                                        wpsjob.CreatedTime = DateTime.Now;
                                        var statusLocation = context.BaseUrl + "/wps/RetrieveResultServlet?id=" + wpsjob.Identifier;
                                        wpsjob.StatusLocation = statusLocation;
                                        wpsjob.Parameters = inputparams.ToList();
                                        wpsjob.Store();
                                        executeResponse.statusLocation = statusLocation;
                                        executeResponse.Status = new StatusType{ Item = new ProcessAcceptedType() };    
                                    }catch(Exception e) {
                                        throw e;
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        throw new Exception("Invalid identifier");
                }
                Stream stream = new System.IO.MemoryStream();

                new System.Xml.Serialization.XmlSerializer(typeof(OpenGis.Wps.ExecuteResponse)).Serialize(stream, executeResponse);
                return new HttpResult(stream, "application/xml");

            }catch(Exception e){
                return new HttpError(HttpStatusCode.BadRequest, e);
            }
        }

        public object Get(GetResultsServlets request) {
            IfyWebContext context = TepWebContext.GetWebContext(PagePrivileges.EverybodyView);
            context.RestrictedMode = false;
            OpenGis.Wps.ExecuteResponse execResponse = new ExecuteResponse();

            try{
                context.Open();
                //load job from request identifier
                WpsJob wpsjob = WpsJob.FromIdentifier(context, request.Id);
                log.Info(string.Format("Get Job {0} status info",wpsjob.Identifier));

                string statusRemoteUrl = context.GetConfigValue("gisat_statusUrl") + wpsjob.RemoteIdentifier;
                HttpWebRequest executeHttpRequest;
                if (wpsjob.Provider != null){
                    executeHttpRequest = wpsjob.Provider.CreateWebRequest (statusRemoteUrl);
                    executeHttpRequest.Timeout = 15000;
                } else {
                    NetworkCredential credentials = null;
                    var urib = new UriBuilder (statusRemoteUrl);
                    if (!string.IsNullOrEmpty (urib.UserName) && !string.IsNullOrEmpty (urib.Password)) credentials = new NetworkCredential (urib.UserName, urib.Password);
                    executeHttpRequest = WpsProvider.CreateWebRequest (wpsjob.StatusLocation, credentials, context.Username);
                }
                executeHttpRequest.Headers.Remove("REMOTE_USER");
                WpsProcessOffering wps = (WpsProcessOffering)WpsProcessOffering.FromIdentifier(context, wpsjob.ProcessId);
                string responseUrl;
                switch(wps.RemoteIdentifier){
                    case "PUMA":
                        executeHttpRequest.ContentType = "application/json";
                        using (var httpResponse = (HttpWebResponse)executeHttpRequest.GetResponse()){
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                                string result = streamReader.ReadToEnd();                        
                                PumaStatusResponse response = JsonSerializer.DeserializeFromString<PumaStatusResponse>(result);
                                switch(response.status){
                                    case "Started":
                                execResponse.Status = new StatusType{ Item = new ProcessStartedType { Value = "Process started" }, ItemElementName = ItemChoiceType.ProcessStarted };        
                                        break;
                                    case "Processing":
                                execResponse.Status = new StatusType{ Item = new ProcessStartedType { Value = "Process in progress" }, ItemElementName = ItemChoiceType.ProcessStarted };        
                                        break;
                                    case "Finished":
                                execResponse.Status = new StatusType{ Item = new ProcessSucceededType { Value = "Process successful" }, ItemElementName = ItemChoiceType.ProcessSucceeded };        
                                        break;
                                    case "Error":
                                execResponse.Status = new StatusType{ Item = new ProcessFailedType { ExceptionReport = new ExceptionReport ()}, ItemElementName = ItemChoiceType.ProcessFailed };        
                                        break;
                                    default:
                                execResponse.Status = new StatusType{ Item = new ProcessAcceptedType { Value = "Process accepted" }, ItemElementName = ItemChoiceType.ProcessAccepted };        
                                        break;
                                }
                                responseUrl = response.url;
                            }
                        }

                        if(!string.IsNullOrEmpty(responseUrl)){
                            execResponse.ProcessOutputs = new List<OutputDataType>{};
                            execResponse.ProcessOutputs.Add(new OutputDataType{
                                Identifier = new CodeType{ Value = "result_redirect"},
                                Item = new DataType{
                                    Item = new ComplexDataType{
                                        mimeType = "application/xml",
                                        Reference = new OutputReferenceType{
                                            href = responseUrl,
                                            mimeType = "application/html"
                                        }
                                    }
                                }
                            });
                        }
                        execResponse.statusLocation = context.BaseUrl + "/wps/RetrieveResultServlet?id=" + wpsjob.Identifier;
                        System.IO.Stream stream = new System.IO.MemoryStream ();
                        new System.Xml.Serialization.XmlSerializer(typeof(OpenGis.Wps.ExecuteResponse)).Serialize(stream, execResponse);
                        context.Close();
                        return new HttpResult(stream, "application/xml");
                    default:
                        break;
                }

                WpsServiceTep service = new WpsServiceTep ();
                return service.Get (request);

            }catch(Exception e){
                return new HttpResult(e.Message, HttpStatusCode.BadRequest);
            }
        }

    }

    /// <summary>
    /// Puma execute request.
    /// </summary>
    [DataContract]
    public class PumaExecuteRequest {

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [DataMember]
        public string url { get; set; }
    }

    /// <summary>
    /// Puma execute response.
    /// </summary>
    [DataContract]
    public class PumaExecuteResponse {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [DataMember]
        public string id { get; set; }
    }

    /// <summary>
    /// Puma status response.
    /// </summary>
    [DataContract]
    public class PumaStatusResponse {

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [DataMember]
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the source URL.
        /// </summary>
        /// <value>The source URL.</value>
        [DataMember]
        public string sourceUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [DataMember]
        public string url { get; set; }
    }
}

