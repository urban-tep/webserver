using Terradue.Tep.WebServer.Services;
using ServiceStack.ServiceHost;
using Terradue.Tep.WebServer;
using Terradue.Portal;
using ServiceStack.Common.Web;
using System;
using Terradue.WebService.Model;

namespace Terradue.Tep.Urban.WebServer.Services {

    [Route("/support/email", "POST", Summary = "send email from user to support", Notes = "")]
    public class SendEmailFromUserRequestTep {
        [ApiMember(Name = "subject", Description = "email subject", ParameterType = "query", DataType = "string", IsRequired = true)]
        public string subject { get; set; }

        [ApiMember(Name = "body", Description = "email body", ParameterType = "query", DataType = "string", IsRequired = true)]
        public string body { get; set; }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------

    [Api("Tep Urban Terradue webserver")]
    [Restrict(EndpointAttributes.InSecure | EndpointAttributes.InternalNetworkAccess | EndpointAttributes.Json,
              EndpointAttributes.Secure | EndpointAttributes.External | EndpointAttributes.Json)]
    public class EmailSupportServiceTepUrban : ServiceStack.ServiceInterface.Service  {
        
        public object Post(SendEmailFromUserRequestTep request) {
            var context = TepWebContext.GetWebContext(PagePrivileges.UserView);
            try {
                context.Open();
                var user = User.FromId(context, context.UserId);
                context.SendMail(context.GetConfigValue("IT4ISupportEmail"), user.Email, request.subject, request.body);
            } catch (Exception e) { 
                context.Close();
                throw e;
            }
            context.Close();
            return new WebResponseBool(true);
        }
    }
}


