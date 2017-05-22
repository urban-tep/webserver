using System;
using System.IO;
using System.Text;
using System.Xml;
using Funq;
using ServiceStack.ServiceHost;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;
using Terradue.ServiceModel.Syndication;
using Terradue.Tep.Urban.WebServer.Services;
using Terradue.Tep.WebServer;

namespace Terradue.Tep.Urban.WebServer {
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>The Singleton AppHost class. Set initial ServiceStack options and register your web services dependencies and run onload scripts</summary>
    public class AppHost
        : AppHostBase {
        /// <summary>AppHost contructor</summary>
        public AppHost()
            : base("Tep Urban Web Services", typeof(LoginServiceTepUrban).Assembly) {
        }

        /// <summary>Override Configure method</summary>
        public override void Configure(Container container) {
            System.Configuration.Configuration rootWebConfig =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(null);

            log4net.Config.XmlConfigurator.Configure();

            JsConfig.ExcludeTypeInfo = false;
            JsConfig.IncludePublicFields = false;

            //Permit modern browsers (e.g. Firefox) to allow sending of any REST HTTP Method
            var config = new EndpointHostConfig {
                GlobalResponseHeaders = {
                    { "Access-Control-Allow-Origin", "*" }
                    //{ "Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS" }//,
                },
                DebugMode = true, //Enable StackTraces in development
                WebHostUrl = rootWebConfig.AppSettings.Settings["BaseUrl"].Value,
                WriteErrorsToResponse = false, //custom exception handling
                WsdlServiceNamespace = "t2api",
                ReturnsInnerException = false
            };
            config.AllowFileExtensions.Add("json");
            config.AllowFileExtensions.Add("geojson");
            base.SetConfig(config);

            this.ContentTypeFilters.Register("application/opensearchdescription+xml", AppHost.CustomXmlSerializer, null);
            ResponseFilters.Add(CustomResponseFilter);

            this.ServiceExceptionHandler = ExceptionHandling.ServiceExceptionHandler;

        }

        public static void CustomXmlSerializer(IRequestContext reqCtx, object res, IHttpResponse stream) {
            stream.AddHeader("Content-Encoding", Encoding.Default.EncodingName);
            using (XmlWriter writer = XmlWriter.Create(stream.OutputStream, new XmlWriterSettings() {
                OmitXmlDeclaration = false,
                Encoding = Encoding.Default
            })) {
                new System.Xml.Serialization.XmlSerializer(res.GetType()).Serialize(writer, res);
            }
        }

        /// <summary>
        /// Customs the response filter.
        /// </summary>
        public static void CustomResponseFilter(IHttpRequest request, IHttpResponse response, object responseDto) {
            if (request.QueryString["format"] == "rss") {
                response.ContentType = "application/rss+xml";
            }

            if (request.QueryString["format"] == "atom") {
                response.ContentType = "application/atom+xml";
            }

            if (request.QueryString["format"] == "csv") {
                response.ContentType = "text/csv";
            }
        }

        public static void SerializeToStream(IRequestContext requestContext, object response, Stream stream) {
            var syndicationFeed = response as SyndicationFeed;
            if (syndicationFeed == null) return;

            using (XmlWriter xmlWriter = XmlWriter.Create(stream)) {
                Atom10FeedFormatter atomFormatter = new Atom10FeedFormatter(syndicationFeed);
                atomFormatter.WriteTo(xmlWriter);
            }
        }

        public static object DeserializeFromStream(Type type, Stream stream) {
            throw new NotImplementedException();
        }

    }
}

