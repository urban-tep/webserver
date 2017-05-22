using Terradue.Tep.WebServer.Services;
using ServiceStack.ServiceHost;
using Terradue.WebService.Model;
using Terradue.Tep.WebServer;
using Terradue.Portal;
using System;
using Terradue.OpenSearch.Result;
using System.Collections.Generic;
using Terradue.ServiceModel.Syndication;

namespace Terradue.Tep.Urban.WebServer.Services
{
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------

    [Route("/apps/puma", "POST", Summary = "create thematic App for Puma", Notes = "")]
    public class ThematicAppCreateRequestTepUrbanPuma : IReturn<WebResponseBool> {
        [ApiMember(Name = "url", Description = "external url of the thematic app", ParameterType = "query", DataType = "string", IsRequired = true)]
        public string Url { get; set; }
    }

    [Api ("Tep Urban Terradue webserver")]
    [Restrict (EndpointAttributes.InSecure | EndpointAttributes.InternalNetworkAccess | EndpointAttributes.Json,
              EndpointAttributes.Secure | EndpointAttributes.External | EndpointAttributes.Json)]
    public class ThematicAppServiceTepUrban : ThematicAppServiceTep {

        public object Post(ThematicAppCreateRequestTepUrbanPuma request) {
            var context = TepWebContext.GetWebContext(PagePrivileges.UserView);
            try {
                context.Open();
                context.LogInfo(this, string.Format("/apps/puma POST"));
                var username = context.Username;
                var pumaIndex = context.GetConfigValue("puma-apps-index");
                var pumaIdentifier = context.GetConfigValue("puma-apps-identifier") + "-" + username + "-" + DateTime.UtcNow.ToString("d").Replace("/","");
                var pumaTitle = context.GetConfigValue("puma-apps-title") + " - " + username + " - " + DateTime.UtcNow.ToString("d");
                var pumaDescription = context.GetConfigValue("puma-apps-description");
                var pumaIcon = context.GetConfigValue("puma-apps-icon");

                var minLevel = context.GetConfigIntegerValue("appExternalPostUserLevel");
                if (context.UserLevel < minLevel) throw new UnauthorizedAccessException("User is not allowed to create a thematic app");

                //create atom feed
                var feed = new AtomFeed();
                var entries = new List<AtomItem>();
                var atomEntry = new AtomItem();
                var entityType = EntityType.GetEntityType(typeof(ThematicApplication));
                Uri id = new Uri(context.BaseUrl + "/" + entityType.Keyword + "/search?id=" + pumaIdentifier);
                atomEntry = new AtomItem(pumaIdentifier, pumaTitle, null, id.ToString(), DateTime.UtcNow);
                atomEntry.Summary = new TextSyndicationContent(pumaDescription);
                atomEntry.ElementExtensions.Add("identifier", "http://purl.org/dc/elements/1.1/", pumaIdentifier);
                atomEntry.Links.Add(new SyndicationLink(id, "self", pumaTitle, "application/atom+xml", 0));
                if (!string.IsNullOrEmpty(request.Url)) atomEntry.Links.Add(new SyndicationLink(new Uri(request.Url), "alternate", "Thematic app url", "application/html", 0));
                if (!string.IsNullOrEmpty(pumaIcon)) atomEntry.Links.Add(new SyndicationLink(new Uri(pumaIcon), "icon", "Icon url", "image/png", 0));
                entries.Add(atomEntry);
                feed.Items = entries;

                //post to catalogue
                CatalogueFactory.PostAtomFeedToIndex(context, feed, pumaIndex);

                context.Close();
            } catch (Exception e) {
                context.LogError(this, e.Message);
                context.Close();
                throw e;
            }
            return new WebResponseBool(true);
        }

    }
}

