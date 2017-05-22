using System;
using System.Web;
using ServiceStack.Common.Web;



/*!

\addtogroup TepCommunity
@{

    \xrefitem trace_req "Requirement" "Requirement traceability" TS-DES-020 TEP end user rights are enforced by this component with the support of the \ref Authorization component.
    
    \xrefitem trace_req "Requirement" "Requirement traceability" TS-DES-030 TEP data provider rights are enforced by this component with the support of the \ref Authorization component.
    
    \xrefitem trace_req "Requirement" "Requirement traceability" TS-DES-040 TEP Content Authority rights are enforced by this component with the support of the \ref Authorization component.
    
    \xrefitem trace_req "Requirement" "Requirement traceability" TS-DES-050 TEP Resource Administrator rights are enforced by this component with the support of the \ref Authorization component.
    
    \xrefitem trace_req "Requirement" "Requirement traceability" TS-DES-060 TEP expert user rights are enforced by this component with the support of the \ref Authorization component.
    
    \xrefitem trace_req "Requirement" "Requirement traceability" TS-DES-080 TEP ICT provider rights are enforced by this component with the support of the \ref Authorization component.

@}

\addtogroup TepApplication
@{

    \xrefitem trace_req "Requirement" "Requirement traceability" TS-DES-110 Data flow along with services are built into the Thematic Application inside this component.
@}

\addtogroup EOProfile
@{

    \xrefitem trace_req "Requirement" "Requirement traceability" TS-FUN-180 This is the data model defining the Earth Observation profile used in \ref ElasticCas and \ref OpenSearch.
@}

\addtogroup TepData
@{

\xrefitem trace_req "Requirement" "Requirement traceability" TS-FUN-010 The GeoNode and GeoServer export function allows user to upload their data to PUMA.

@}

\addtogroup Geosquare
@{

\xrefitem trace_req "Requirement" "Requirement traceability" TS-FUN-120 Geosquare provides with the Catalogue as a service function

@}

\addtogroup GeosquareAPI
@{

\xrefitem trace_req "Requirement" "Requirement traceability" TS-ICD-090 Geosquare API provides with feed format supporting \ref OWSContext

\xrefitem trace_req "Requirement" "Requirement traceability" TS-ICD-100 Geosquare API provides with an \ref OpenSearch interface

@}

\addtogroup CoreWPS
@{

\xrefitem trace_req "Requirement" "Requirement traceability" TS-FUN-250 This section describes with sequence diagrams the internal WPS service discovery

@}

\addtogroup Authorisation
@{

\xrefitem trace_req "Requirement" "Requirement traceability" TS-FUN-380 Authorisation scheme paradigm is described in this section.

\xrefitem trace_req "Requirement" "Requirement traceability" TS-SEC-010 Collection / Series / Data Package Authorisation scheme paradigm is described in this section.

\xrefitem trace_req "Requirement" "Requirement traceability" TS-SEC-020 Service Authorisation scheme paradigm is described in this section.

@}

\addtogroup Auth_Umsso
@{

\xrefitem trace_req "Requirement" "Requirement traceability" TS-SEC-040 UM-SSO plugin described
@}

\addtogroup TepAccounting
@{

\xrefitem trace_req "Requirement" "Requirement traceability" TS-FUN-400 The mechanism of debit and credit of user or group accounts is described in this section.

\xrefitem trace_req "Requirement" "Requirement traceability" TS-FUN-410 Business objects used to represent user or group account are referenced in this section.

\xrefitem trace_req "Requirement" "Requirement traceability" TS-FUN-420 Quota mechanism based on the account balance of the user or groups is described in this section.

@}

\addtogroup T2API
@{

\xrefitem trace_req "Requirement" "Requirement traceability" TS-ICD-020 OGC \ref OWSContext is used as the most as possible for representing objects in the portal

\xrefitem trace_req "Requirement" "Requirement traceability" TS-ICD-100 T2 API provides with an \ref OpenSearch interface

@}


\addtogroup ApelReporting
@{

\xrefitem trace_req "Requirement" "Requirement traceability" TS-ICD-070 Apel reporting interface details are described in this section

@}


\addtogroup ApelAccounting
@{

\xrefitem trace_req "Requirement" "Requirement traceability" TS-ICD-080 Apel accounting interface details are described in this section

@}

\addtogroup Series
@{

\xrefitem trace_req "Requirement" "Requirement traceability" TS-ICD-090 This component reads dataset series metadata using
OWS conceptual model.

\xrefitem trace_req "Requirement" "Requirement traceability" TS-ICD-100 \ref OpenSearch interface for dataset are supported by the component

@}

*/



namespace Terradue.Tep.Urban.WebServer {

    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    /// <summary>Global class</summary>
    public class Global : HttpApplication {
        public AppHost appHost;

        /// <summary>Application initialisation</summary>
        protected void Application_Start(object sender, EventArgs e) {
            appHost = new AppHost();
            appHost.Init();
        }

        protected void Application_Error(object sender, EventArgs e) {
            Context.IsErrorResponse();
        }

        protected void Application_BeginRequest(object sender, EventArgs e) {
            string urlPath = Request.Path.ToLower();
        }
    }
}
