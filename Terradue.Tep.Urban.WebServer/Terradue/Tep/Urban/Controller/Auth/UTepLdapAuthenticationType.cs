using System;
using System.Web;
using Terradue.Portal;

namespace Terradue.Tep.Urban {

    public class UTepLdapAuthenticationType : TepLdapAuthenticationType {
        public UTepLdapAuthenticationType(IfyContext context) : base(context) { }

        public override User GetUserProfile(IfyWebContext context, HttpRequest request = null, bool strict = false) {
            var user = base.GetUserProfile(context, request, strict);

            if (this.NewUserCreated) {
                // new user has been created

                // send information email to user
                context.LogInfo(this, string.Format("New user created {0} - sending onboarding information", user.Username));

                var subject = context.GetConfigValue("onboardingEmail_subject");
                var body = context.GetConfigValue("onboardingEmail_body");
                var username = !string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName) ? user.FirstName + " " + user.LastName : user.Username;
                body = body.Replace("$(USERNAME)", username);

                try {
                    context.SendMail(context.GetConfigValue("IT4ISupportEmail"), user.Email, subject, body);
                } catch (Exception e) {
                    context.LogError(this, "Unable to send email to add user to support system: " + e.Message);
                }

                // we add him to redmine support
                context.LogInfo(this, string.Format("New user created {0} - adding to support system", user.Username));

                subject = context.GetConfigValue("addtosupportEmail_subject");
                body = context.GetConfigValue("addtosupportEmail_body");
                body = body.Replace("$(USERNAME)", user.Username);

                try {
                    context.SendMail(user.Email, context.GetConfigValue("IT4ISupportEmail"), subject, body);
                } catch (Exception e) {
                    context.LogError(this, "Unable to send email to add user to support system: " + e.Message);
                }
            }

            return user;
        }
    }
}
