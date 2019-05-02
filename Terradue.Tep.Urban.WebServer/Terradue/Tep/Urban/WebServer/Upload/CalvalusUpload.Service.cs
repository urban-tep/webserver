using System;
using System.Collections.Generic;
using System.IO;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using Terradue.Portal;
using Terradue.Tep.WebServer;
using Terradue.WebService.Model;

namespace Terradue.Tep.Urban.WebServer {

    [Route("/utep/processor-packages", "GET", Summary = "GET the processor package", Notes = "")]
    public class ProcessorPackageGetRequestTepUrban : IReturn<HttpResult> {
    }

    [Route("/utep/processor-packages", "POST", Summary = "POST the processor package", Notes = "")]
    public class ProcessorPackagePostRequestTepUrban : IRequiresRequestStream, IReturn<HttpResult> {
        [ApiMember(Name = "RequestStream", Description = "RequestStream", ParameterType = "query", DataType = "Stream", IsRequired = false)]
        public System.IO.Stream RequestStream { get; set; }
    }

    [Route("/utep/processor-packages", "GET", Summary = "GET the processor package info", Notes = "")]
    public class ProcessorPackageInfoGetRequestTepUrban : IReturn<HttpResult> {
        [ApiMember(Name = "name", Description = "package name", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Name { get; set; }
    }

    [Route("/utep/processor-packages", "DELETE", Summary = "DELETE the processor package", Notes = "")]
    public class ProcessorPackageDeleteRequestTepUrban : IReturn<HttpResult> {
        [ApiMember(Name = "name", Description = "package name", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Name { get; set; }
    }

    [Route("/utep/processor-packages/content", "GET", Summary = "GET the processor package content", Notes = "")]
    public class ProcessorPackageContentGetRequestTepUrban : IReturn<HttpResult> {
        [ApiMember(Name = "name", Description = "package name", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Name { get; set; }
    }

    [Route("/utep/processor-packages/files", "GET", Summary = "GET the processor package files", Notes = "")]
    public class ProcessorPackageFilesGetRequestTepUrban : IReturn<HttpResult> {
        [ApiMember(Name = "name", Description = "package name", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Name { get; set; }
    }

    [Route("/utep/processor-packages/files", "POST", Summary = "POST the processor package files", Notes = "")]
    public class ProcessorPackageFilesPostRequestTepUrban : IRequiresRequestStream, IReturn<HttpResult> {
        [ApiMember(Name = "RequestStream", Description = "RequestStream", ParameterType = "query", DataType = "Stream", IsRequired = false)]
        public System.IO.Stream RequestStream { get; set; }

        [ApiMember(Name = "name", Description = "package name", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Name { get; set; }
    }

    [Route("/utep/processor-packages/files", "GET", Summary = "GET the processor package file info", Notes = "")]
    public class ProcessorPackageFileInfoGetRequestTepUrban : IReturn<HttpResult> {
        [ApiMember(Name = "name", Description = "package name", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Name { get; set; }

        [ApiMember(Name = "filename", Description = "package filename", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Filename { get; set; }
    }

    [Route("/utep/processor-packages/files", "DELETE", Summary = "DELETE the processor package file info", Notes = "")]
    public class ProcessorPackageFileDeleteRequestTepUrban : IReturn<HttpResult> {
        [ApiMember(Name = "name", Description = "package name", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Name { get; set; }

        [ApiMember(Name = "filename", Description = "package filename", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Filename { get; set; }
    }

    [Route("/utep/processor-packages/files/content", "GET", Summary = "GET the processor package file content", Notes = "")]
    public class ProcessorPackageFileContentGetRequestTepUrban : IReturn<HttpResult> {
        [ApiMember(Name = "name", Description = "package name", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Name { get; set; }

        [ApiMember(Name = "filename", Description = "package filename", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Filename { get; set; }
    }

    [Route("/utep/shapefiles", "GET", Summary = "GET the shapefiles", Notes = "")]
    public class ShapefileGetRequestTepUrban : IReturn<HttpResult> {
    }

    [Route("/utep/shapefiles", "POST", Summary = "POST the shapefiles", Notes = "")]
    public class ShapefilePostRequestTepUrban : IRequiresRequestStream, IReturn<HttpResult> {
        [ApiMember(Name = "RequestStream", Description = "RequestStream", ParameterType = "query", DataType = "Stream", IsRequired = false)]
        public System.IO.Stream RequestStream { get; set; }
    }

    [Route("/utep/shapefiles", "GET", Summary = "GET the shapefiles", Notes = "")]
    public class ShapefileGetFilesRequestTepUrban : IReturn<HttpResult> {
        [ApiMember(Name = "filename", Description = "package filename", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Filename { get; set; }
    }

    [Route("/utep/shapefiles", "DELETE", Summary = "DELETE the shapefiles", Notes = "")]
    public class ShapefileDeleteRequestTepUrban : IReturn<HttpResult> {
        [ApiMember(Name = "Name", Description = "shapefile name", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Name { get; set; }
    }

    [Api("Tep Urban Terradue webserver")]
    [Restrict(EndpointAttributes.InSecure | EndpointAttributes.InternalNetworkAccess | EndpointAttributes.Json,
              EndpointAttributes.Secure | EndpointAttributes.External | EndpointAttributes.Json)]
    public class WpsServiceTepUrban : ServiceStack.ServiceInterface.Service {

        public object Get(ProcessorPackageGetRequestTepUrban request) {
            IfyWebContext context;
            List<ProcessorPackage> response = null;
            context = TepWebContext.GetWebContext(PagePrivileges.DeveloperView);
            context.LogInfo(this, string.Format("/utep/processor-packages GET"));
            context.Open();
            try {
                var calvalusClient = new CalvalusUploadClient(context.GetConfigValue("calvalusApi-baseUrl"), context.GetConfigValue("calvalusApi-username"), context.GetConfigValue("calvalusApi-password"));
                response = calvalusClient.ProcessorPackageGetList(context.Username);
            } catch(Exception e){
                context.LogError(this, e.Message + " - " + e.StackTrace);
                context.Close();
                throw e;
            }
            context.Close();
            return response;
        }

        public object Post(ProcessorPackagePostRequestTepUrban request) {
            IfyWebContext context;
            context = TepWebContext.GetWebContext(PagePrivileges.DeveloperView);
            context.LogInfo(this, string.Format("/utep/processor-packages POST"));
            context.Open();
            string path = System.Configuration.ConfigurationManager.AppSettings["UploadTmpPath"] ?? "/tmp";
            try{
                var calvalusClient = new CalvalusUploadClient(context.GetConfigValue("calvalusApi-baseUrl"), context.GetConfigValue("calvalusApi-username"), context.GetConfigValue("calvalusApi-password"));
                var filename = path + "/" + Guid.NewGuid().ToString() + ".zip";
                using (var stream = new MemoryStream()) {
                    if (this.RequestContext.Files.Length > 0) {
                        var uploadedFile = this.RequestContext.Files[0];
                        filename = path + "/" + this.RequestContext.Files[0].FileName;
                        uploadedFile.SaveTo(filename);
                    } else {
                        using (var fileStream = File.Create(filename)) {
                            request.RequestStream.CopyTo(fileStream);
                        }
                    }
                    calvalusClient.ProcessorPackageUpload(context.Username, filename);
                }
            } catch (Exception e) {
                context.LogError(this, e.Message + " - " + e.StackTrace);
                context.Close();
                throw e;
            }
            context.Close();
            return new WebResponseBool(true);
        }

        public object Get(ProcessorPackageInfoGetRequestTepUrban request) {
            IfyWebContext context;
            ProcessorPackage response = null;
            context = TepWebContext.GetWebContext(PagePrivileges.DeveloperView);
            context.LogInfo(this, string.Format("/utep/processor-packages/{0} GET", request.Name));
            context.Open();
            try{
                var calvalusClient = new CalvalusUploadClient(context.GetConfigValue("calvalusApi-baseUrl"), context.GetConfigValue("calvalusApi-username"), context.GetConfigValue("calvalusApi-password"));
                response = calvalusClient.ProcessorPackageGetInfo(request.Name, context.Username);
            } catch (Exception e) {
                context.LogError(this, e.Message + " - " + e.StackTrace);
                context.Close();
                throw e;
            }

            context.Close();
            return response;
        }

        public object Delete(ProcessorPackageDeleteRequestTepUrban request) {
            IfyWebContext context;
            context = TepWebContext.GetWebContext(PagePrivileges.DeveloperView);
            context.LogInfo(this, string.Format("/utep/processor-packages/{0} DELETE", request.Name));
            context.Open();
            try{
                var calvalusClient = new CalvalusUploadClient(context.GetConfigValue("calvalusApi-baseUrl"), context.GetConfigValue("calvalusApi-username"), context.GetConfigValue("calvalusApi-password"));
                calvalusClient.ProcessorsPackageDelete(request.Name, context.Username);
            } catch (Exception e) {
                context.LogError(this, e.Message + " - " + e.StackTrace);
                context.Close();
                throw e;
            }
            context.Close();
            return new WebResponseBool(true);
        }

        public object Get(ProcessorPackageContentGetRequestTepUrban request) {
            IfyWebContext context;
            byte[] response = null;
            context = TepWebContext.GetWebContext(PagePrivileges.DeveloperView);
            context.LogInfo(this, string.Format("/utep/processor-packages/{0}/content GET", request.Name));
            context.Open();
            try{
                var calvalusClient = new CalvalusUploadClient(context.GetConfigValue("calvalusApi-baseUrl"), context.GetConfigValue("calvalusApi-username"), context.GetConfigValue("calvalusApi-password"));
                response = (byte[])calvalusClient.ProcessorPackageGetContent(request.Name, context.Username);
            } catch (Exception e) {
                context.LogError(this, e.Message + " - " + e.StackTrace);
                context.Close();
                throw e;
            }
            context.Close();
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.zip", request.Name));
            return new HttpResult( new MemoryStream(response), "application/zip");
        }

        public object Get(ProcessorPackageFilesGetRequestTepUrban request) {
            IfyWebContext context;
            List<ProcessorPackageFile> response = null;
            context = TepWebContext.GetWebContext(PagePrivileges.DeveloperView);
            context.LogInfo(this, string.Format("/utep/processor-packages/{0}/files GET", request.Name));
            context.Open();
            try{
                var calvalusClient = new CalvalusUploadClient(context.GetConfigValue("calvalusApi-baseUrl"), context.GetConfigValue("calvalusApi-username"), context.GetConfigValue("calvalusApi-password"));
                response = calvalusClient.ProcessorPackageGetFiles(request.Name, context.Username);
            } catch (Exception e) {
                context.LogError(this, e.Message + " - " + e.StackTrace);
                context.Close();
                throw e;
            }
            context.Close();
            return response;
        }

        public object Post(ProcessorPackageFilesPostRequestTepUrban request) {
            IfyWebContext context;
            context = TepWebContext.GetWebContext(PagePrivileges.DeveloperView);
            context.LogInfo(this, string.Format("/utep/processor-packages/{0}/files POST", request.Name));
            context.Open();
            try{
                var calvalusClient = new CalvalusUploadClient(context.GetConfigValue("calvalusApi-baseUrl"), context.GetConfigValue("calvalusApi-username"), context.GetConfigValue("calvalusApi-password"));
                using (var stream = new MemoryStream()) {
                    if (this.RequestContext.Files.Length > 0)
                        this.RequestContext.Files[0].InputStream.CopyTo(stream);
                    else
                        request.RequestStream.CopyTo(stream);
                    calvalusClient.ProcessorPackageFileUpdate(request.Name, context.Username, stream);
                }
            } catch (Exception e) {
                context.LogError(this, e.Message + " - " + e.StackTrace);
                context.Close();
                throw e;
            }
            context.Close();
            return new WebResponseBool(true);
        }

        public object Get(ProcessorPackageFileInfoGetRequestTepUrban request) {
            IfyWebContext context;
            ProcessorPackageFile response = null;
            context = TepWebContext.GetWebContext(PagePrivileges.DeveloperView);
            context.LogInfo(this, string.Format("/utep/processor-packages/{0}/files/{1} GET", request.Name, request.Filename));
            context.Open();
            try {
                var calvalusClient = new CalvalusUploadClient(context.GetConfigValue("calvalusApi-baseUrl"), context.GetConfigValue("calvalusApi-username"), context.GetConfigValue("calvalusApi-password"));
                response = calvalusClient.ProcessorPackageGetFileMetadata(request.Name, request.Filename, context.Username);
            } catch (Exception e) {
                context.LogError(this, e.Message + " - " + e.StackTrace);
                context.Close();
                throw e;
            }
            context.Close();
            return response;
        }

        public object Delete(ProcessorPackageFileDeleteRequestTepUrban request) {
            IfyWebContext context;
            context = TepWebContext.GetWebContext(PagePrivileges.DeveloperView);
            context.LogInfo(this, string.Format("/utep/processor-packages/{0}/files/{1} DELETE", request.Name, request.Filename));
            context.Open();
            try {
                var calvalusClient = new CalvalusUploadClient(context.GetConfigValue("calvalusApi-baseUrl"), context.GetConfigValue("calvalusApi-username"), context.GetConfigValue("calvalusApi-password"));
                calvalusClient.ProcessorPackageDeleteFile(request.Name, request.Filename, context.Username);
            } catch (Exception e) {
                context.LogError(this, e.Message + " - " + e.StackTrace);
                context.Close();
                throw e;
            }
            context.Close();
            return new WebResponseBool(true);
        }

        public object Get(ProcessorPackageFileContentGetRequestTepUrban request) {
            IfyWebContext context;
            byte[] response = null;
            context = TepWebContext.GetWebContext(PagePrivileges.DeveloperView);
            context.LogInfo(this, string.Format("/utep/processor-packages/{0}/files/{1}/content GET", request.Name, request.Filename));
            context.Open();
            try {
                var calvalusClient = new CalvalusUploadClient(context.GetConfigValue("calvalusApi-baseUrl"), context.GetConfigValue("calvalusApi-username"), context.GetConfigValue("calvalusApi-password"));
                response = (byte[])calvalusClient.ProcessorPackageGetFileContent(request.Name, request.Filename, context.Username);
            } catch (Exception e) {
                context.LogError(this, e.Message + " - " + e.StackTrace);
                context.Close();
                throw e;
            }
            context.Close();
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", request.Filename));
            return new HttpResult(new MemoryStream(response), "application/xml");
        }

        public object Get(ShapefileGetRequestTepUrban request) {
            IfyWebContext context;
            List<ShapefilePackage> response = null;
            context = TepWebContext.GetWebContext(PagePrivileges.DeveloperView);
            context.LogInfo(this, string.Format("/utep/shapefiles GET"));
            context.Open();
            try {
                var calvalusClient = new CalvalusUploadClient(context.GetConfigValue("calvalusApi-baseUrl"), context.GetConfigValue("calvalusApi-username"), context.GetConfigValue("calvalusApi-password"));
                response = calvalusClient.ShapefilesGetList(context.Username);
            } catch (Exception e) {
                context.LogError(this, e.Message + " - " + e.StackTrace);
                context.Close();
                throw e;
            }
            context.Close();
            return response;
        }

        public object Post(ShapefilePostRequestTepUrban request) {
            IfyWebContext context;
            context = TepWebContext.GetWebContext(PagePrivileges.DeveloperView);
            context.LogInfo(this, string.Format("/utep/shapefiles POST"));
            context.Open();
            string path = System.Configuration.ConfigurationManager.AppSettings["UploadTmpPath"] ?? "/tmp";
            try {
                var calvalusClient = new CalvalusUploadClient(context.GetConfigValue("calvalusApi-baseUrl"), context.GetConfigValue("calvalusApi-username"), context.GetConfigValue("calvalusApi-password"));
                var filename = path + "/" + Guid.NewGuid().ToString() + ".zip";
                using (var stream = new MemoryStream()) {
                    if (this.RequestContext.Files.Length > 0) {
                        var uploadedFile = this.RequestContext.Files[0];
                        filename = path + "/" + this.RequestContext.Files[0].FileName;
                        uploadedFile.SaveTo(filename);
                    } else {
                        using (var fileStream = File.Create(filename)) {
                            request.RequestStream.CopyTo(fileStream);
                        }
                    }
                    calvalusClient.ShapefileUpload(context.Username, filename);
                }
            } catch (Exception e) {
                context.LogError(this, e.Message + " - " + e.StackTrace);
                context.Close();
                throw e;
            }
            context.Close();
            return new WebResponseBool(true);
        }

        public object Get(ShapefileGetFilesRequestTepUrban request) {
            IfyWebContext context;
            ShapefilePackage response = null;
            context = TepWebContext.GetWebContext(PagePrivileges.DeveloperView);
            context.LogInfo(this, string.Format("/utep/shapefiles/{0} GET", request.Filename));
            context.Open();
            try {
                var calvalusClient = new CalvalusUploadClient(context.GetConfigValue("calvalusApi-baseUrl"), context.GetConfigValue("calvalusApi-username"), context.GetConfigValue("calvalusApi-password"));
                response = calvalusClient.ShapefileGetInfo(request.Filename, context.Username);
            } catch (Exception e) {
                context.LogError(this, e.Message + " - " + e.StackTrace);
                context.Close();
                throw e;
            }

            context.Close();
            return response;
        }

        public object Delete(ShapefileDeleteRequestTepUrban request) {
            IfyWebContext context;
            context = TepWebContext.GetWebContext(PagePrivileges.DeveloperView);
            context.LogInfo(this, string.Format("/utep/shapefiles/{0} DELETE", request.Name));
            context.Open();
            try {
                var calvalusClient = new CalvalusUploadClient(context.GetConfigValue("calvalusApi-baseUrl"), context.GetConfigValue("calvalusApi-username"), context.GetConfigValue("calvalusApi-password"));
                calvalusClient.ShapefileDelete(request.Name, context.Username);
            } catch (Exception e) {
                context.LogError(this, e.Message + " - " + e.StackTrace);
                context.Close();
                throw e;
            }
            context.Close();
            return new WebResponseBool(true);
        }
    }

}
