using Terradue.Tep.WebServer.Services;
using ServiceStack.ServiceHost;

namespace Terradue.Tep.Urban.WebServer.Services {
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------

    [Api("Tep Urban Terradue webserver")]
    [Restrict(EndpointAttributes.InSecure | EndpointAttributes.InternalNetworkAccess | EndpointAttributes.Json,
              EndpointAttributes.Secure | EndpointAttributes.External | EndpointAttributes.Json)]
    public class RoleServiceTepUrban : RoleServiceTep { }
}

