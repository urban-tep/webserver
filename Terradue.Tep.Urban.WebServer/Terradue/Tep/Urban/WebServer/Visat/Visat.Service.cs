using System;
using System.Collections.Generic;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using Terradue.Portal;
using Terradue.Tep.WebServer;

namespace Terradue.Tep.Urban.WebServer.Visat {

    [Route("/private/visat/community", "POST", Summary = "Get communities for VISAT", Notes = "")]
    public class VisatGetCommunitiesTepUrban : IReturn<List<WebCommunityVisat>> {
        [ApiMember(Name = "apikey", Description = "Api key of Visat", ParameterType = "query", DataType = "string", IsRequired = true)]
        public string ApiKey { get; set; }

        [ApiMember(Name = "username", Description = "user identifier", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Username { get; set; }
    }

    public class WebCommunityVisat {

        [ApiMember(Name = "Identifier", Description = "Identifier", ParameterType = "query", DataType = "string", IsRequired = true)]
        public string Identifier { get; set; }

        [ApiMember(Name = "Name", Description = "Name", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Name { get; set; }

        [ApiMember(Name = "Description", Description = "Description", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string Description { get; set; }

        [ApiMember(Name = "Icon", Description = "Icon", ParameterType = "query", DataType = "string", IsRequired = false)]
        public string IconUrl { get; set; }
    }

        [Api("Tep Urban Terradue webserver")]
    [Restrict(EndpointAttributes.InSecure | EndpointAttributes.InternalNetworkAccess | EndpointAttributes.Json,
              EndpointAttributes.Secure | EndpointAttributes.External | EndpointAttributes.Json)]
    public class VisatService : ServiceStack.ServiceInterface.Service {

        public object Post(VisatGetCommunitiesTepUrban request) {
            IfyWebContext context;
            System.IO.Stream stream = new System.IO.MemoryStream();
            context = TepWebContext.GetWebContext(PagePrivileges.EverybodyView);

            context.Open();

            var key = context.GetConfigValue("visat_apikey");
            if (string.IsNullOrEmpty(request.ApiKey) || request.ApiKey != key) throw new Exception("Invalid token");

            UserTep user = null;
            if (!string.IsNullOrEmpty(request.Username)) {
                try {
                    user = UserTep.FromIdentifier(context, request.Username);
                } catch (Exception e) {
                    throw new Exception("Invalid username");
                }
            }

            //get all communties
            EntityList<ThematicCommunity> communities = new EntityList<ThematicCommunity>(context);
            communities.SetFilter("Kind", (int)DomainKind.Public + "," + (int)DomainKind.Private + "," + (int)DomainKind.Hidden);
            communities.Load();

            var result = new List<WebCommunityVisat>();
            if (communities.Items != null) {
                foreach (var item in communities.Items) {
                    if (user == null || item.IsUserJoined(user.Id)) {
                        result.Add(new WebCommunityVisat { Identifier = item.Identifier, Name = item.Name, Description = item.Description, IconUrl = item.IconUrl });
                    }
                }
            }
            return result;
        }
    }
}