using ServiceStack.ServiceHost;
using Terradue.Tep.WebServer.Services;

namespace Terradue.Tep.Urban.WebServer.Services
{
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------

    [Api("Tep Urban Terradue webserver")]
    [Restrict(EndpointAttributes.InSecure | EndpointAttributes.InternalNetworkAccess | EndpointAttributes.Json,
              EndpointAttributes.Secure   | EndpointAttributes.External | EndpointAttributes.Json)]
    public class RateServiceTepUrban : RatesServiceTep {}
}

