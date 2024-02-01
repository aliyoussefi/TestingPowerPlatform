using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization.Json;
using System.Security;
using System.ServiceModel.Description;
using System.Text;
using static D365.UnitTests.Copilot.CopilotTesting;

namespace D365.UnitTests.Copilot
{
    [TestClass]
    public class CustomerServiceCopilot
    {
        private static string _dataverseServiceClientConnection;
        private static string _crmServiceClientConnection;
        private static string _clientId;
        private static string _clientSecret;
        private static string _tenantId;
        private static string _orgUrl;
        private static TestContext _testContext;

        [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
        public static void Initialize(TestContext TestContext)
        {
            _testContext = TestContext;
            _dataverseServiceClientConnection = _testContext.Properties["DataverseServiceClientConnection"].ToString();
            _crmServiceClientConnection = _testContext.Properties["CrmServiceClientConnection"].ToString();

            _clientId = _testContext.Properties["clientId"].ToString();
            _clientSecret = _testContext.Properties["clientSecret"].ToString();
            _tenantId = _testContext.Properties["tenantId"].ToString();
            _orgUrl = _testContext.Properties["orgUrl"].ToString();
        }
        #region Invoke Intelligence Action Classes
        public class InvokeIntelligenceActionRequest
        {
            public string ScenarioType { get; set; }
            public InvokeIntelligenceActionRequestPayload RequestPayload { get; set; }
            public InvokeIntelligenceActionRequestContext Context { get; set; }
        }

        public class InvokeIntelligenceActionRequestPayload
        {
            public string incidentid { get; set; }
            public bool allowcrossgeo { get; set; }
            public string localizationCode { get; set; }
            public string input_request_type { get; set; }
            public string output_mode { get; set; }
            public InvokeIntelligenceActionRequestPayloadContent content { get; set; }
        }

        public class InvokeIntelligenceActionRequestPayloadContent
        {
            public List<InvokeIntelligenceActionRequestPayloadContentMessage> messages { get; set; }
        }
        public class InvokeIntelligenceActionRequestPayloadContentMessage
        {
            public string user { get; set; }
            public string text { get; set; }
            public string datetime { get; set; }
        }

        public class InvokeIntelligenceActionRequestContext
        {
            public string scenariorequestid { get; set; }
            public string regardingobjectid { get; set; }
            public bool verifyfullaccess { get; set; }
        }
        #endregion
        
        [TestCategory("CustomerServiceCopilot")]
        [TestMethod]
        public void msdyn_InvokeIntelligenceAction_SDK_SystemUser_RespondWithKnowledgeArticle()
        {

            Microsoft.PowerPlatform.Dataverse.Client.ServiceClient serviceClient = new Microsoft.PowerPlatform.Dataverse.Client.ServiceClient(_dataverseServiceClientConnection);


            var req = new OrganizationRequest("msdyn_InvokeIntelligenceAction")
            {
                ["Target"] = new EntityReference("incident", Guid.NewGuid()),
                ["Priority"] = new OptionSetValue(1)
            };

            var resp = serviceClient.Execute(req);

            var newOwner = (EntityReference)resp["AssignedTo"];
        }

        [TestCategory("CustomerServiceCopilot")]
        [TestMethod]
        public void msdyn_InvokeIntelligenceAction_SDK_CrmServiceClient_SystemUser_RespondWithKnowledgeArticle()
        {

            CrmServiceClient serviceClient = new CrmServiceClient(_crmServiceClientConnection);


            var req = new OrganizationRequest("msdyn_InvokeIntelligenceAction")
            {
                ["Target"] = new EntityReference("incident", Guid.NewGuid()),
                ["Priority"] = new OptionSetValue(1)
            };

            var resp = serviceClient.Execute(req);

            var newOwner = (EntityReference)resp["AssignedTo"];
        }

        [TestCategory("CustomerServiceCopilot")]
        [TestCategory("msdyn_InvokeIntelligenceAction")]
        [TestCategory("API")]
        [TestMethod]
        public void msdyn_InvokeIntelligenceAction_API_SystemUserToken_WillFail()
        {
            #region auth
            string resource = "https://orgb66e00bc.api.crm.dynamics.com";
            var clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
            var redirectUri = "http://localhost"; // Loopback for the interactive login.
            var methodName = "msdyn_InvokeIntelligenceAction";

            // MSAL authentication
            var authBuilder = PublicClientApplicationBuilder.Create(clientId)
                .WithAuthority(AadAuthorityAudience.AzureAdMultipleOrgs)
                .WithRedirectUri(redirectUri)
                .Build();
            var scope = resource + "/user_impersonation";
            string[] scopes = { scope };

            Microsoft.Identity.Client.AuthenticationResult token =
                authBuilder.AcquireTokenInteractive(scopes).ExecuteAsync().Result;
            #endregion

            #region build http
            // Set up the HTTP client
            var client = new HttpClient
            {
                BaseAddress = new Uri(resource + "/api/data/v9.2/"),
                Timeout = new TimeSpan(0, 2, 0)  // Standard two minute timeout.
            };

            HttpRequestHeaders headers = client.DefaultRequestHeaders;
            headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            headers.Add("OData-MaxVersion", "4.0");
            headers.Add("OData-Version", "4.0");
            headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            headers.Add("OData-MaxVersion", "4.0");
            headers.Add("OData-Version", "4.0");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://orgb66e00bc.api.crm.dynamics.com/api/data/v9.2/" + methodName);


            headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            #endregion

            #region payload
            InvokeIntelligenceActionRequest requestPayload = new InvokeIntelligenceActionRequest();
            requestPayload.ScenarioType = "IntentIdentification";
            requestPayload.RequestPayload = new InvokeIntelligenceActionRequestPayload();
            requestPayload.RequestPayload.input_request_type = "copilot_transcript";
            requestPayload.RequestPayload.output_mode = "intent";
            requestPayload.RequestPayload.content = new InvokeIntelligenceActionRequestPayloadContent();
            requestPayload.RequestPayload.content.messages = new List<InvokeIntelligenceActionRequestPayloadContentMessage>();
            requestPayload.RequestPayload.content.messages.Add(new InvokeIntelligenceActionRequestPayloadContentMessage()
            {
                text = "how should i respond to this customers?",
                user = "customer",
                datetime = DateTime.Now.ToString()
            });

            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(InvokeIntelligenceActionRequest), new DataContractJsonSerializerSettings()
            {
                UseSimpleDictionaryFormat = true
            });
            ser.WriteObject(stream1, requestPayload);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);

            #endregion


            // Web API call
            var myContent = JsonConvert.SerializeObject(requestPayload);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            request.Content = new StringContent(myContent,
                                    Encoding.UTF8,
                                    "application/json");//CONTENT-TYPE header

            client.SendAsync(request)
                  .ContinueWith(responseTask =>
                  {
                      Console.WriteLine("Response: {0}", responseTask.Result);
                  }).GetAwaiter().GetResult();
        }
    }
}