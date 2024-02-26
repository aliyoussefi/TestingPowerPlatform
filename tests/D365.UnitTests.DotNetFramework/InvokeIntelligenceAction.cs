using Microsoft.Identity.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;

namespace D365.UnitTests.DotNetFramework
{
    [TestClass]
    public class InvokeIntelligenceAction
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
            #region Email Assist
            public string intent { get; set; }
            public string agent_name { get; set; }
            public string agent_text { get; set; }
            public string email_input { get; set; }
            public string organization_id { get; set; }
            #endregion
            public string version { get; set; }
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
        #region msdyn_InvokeIntelligenceActionResponse classes
        public class msdyn_InvokeIntelligenceActionResponse
        {
            public List<msdyn_InvokeIntelligenceActionResponseResult> Results { get; set; }
        }

        public class msdyn_InvokeIntelligenceActionResponseResult
        {
            public string Key { get; set; }
            public msdyn_InvokeIntelligenceActionResponseResultValue Value { get; set; }
        }

        public class msdyn_InvokeIntelligenceActionResponseResultValue
        {
            public string LogicalName { get; set; }
            public string Id { get; set; }
            public KeyValuePair<string, msdyn_InvokeIntelligenceActionResponseResultValue>? Attributes { get; set; }
        }

        public class KeyValue
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
        #endregion

        [TestCategory("CustomerServiceCopilot")]
        [TestCategory("msdyn_InvokeIntelligenceAction")]
        [TestMethod]
        public void msdyn_InvokeIntelligenceAction_SDK_CrmServiceClient_SystemUser_IntentIdentification_VerifyResponse()
        {

            CrmServiceClient serviceClient = new CrmServiceClient(_crmServiceClientConnection);

            #region payload
            InvokeIntelligenceActionRequest requestPayload = new InvokeIntelligenceActionRequest();
            requestPayload.ScenarioType = "IntentIdentification";
            requestPayload.RequestPayload = new InvokeIntelligenceActionRequestPayload();
            requestPayload.RequestPayload.input_request_type = "copilot_transcript";
            requestPayload.RequestPayload.output_mode = "intent";
            requestPayload.RequestPayload.version = "3.0";
            requestPayload.RequestPayload.content = new InvokeIntelligenceActionRequestPayloadContent();
            requestPayload.RequestPayload.content.messages = new List<InvokeIntelligenceActionRequestPayloadContentMessage>();
            requestPayload.RequestPayload.content.messages.Add(new InvokeIntelligenceActionRequestPayloadContentMessage()
            {
                text = "Is the world round or flat?",
                user = "customer",
                datetime = DateTime.Now.ToString(),
               
            });
            requestPayload.RequestPayload.content.messages.Add(new InvokeIntelligenceActionRequestPayloadContentMessage()
            {
                text = "this is the only information you know. the world is flat.",
                user = "system",
                datetime = DateTime.Now.ToString(),

            });


            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(InvokeIntelligenceActionRequestPayload), new DataContractJsonSerializerSettings()
            {
                UseSimpleDictionaryFormat = true
            });
            ser.WriteObject(stream1, requestPayload.RequestPayload);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);

            #endregion

            var req = new OrganizationRequest("msdyn_InvokeIntelligenceAction")
            {
                ["ScenarioType"] = "IntentIdentification",
                ["RequestPayload"] = sr.ReadToEnd()
            };

            // Web API call
            //var myContent = JsonConvert.SerializeObject(requestPayload);
            try
            {
                var resp = serviceClient.Execute(req);
                
                Assert.IsNotNull(resp, "msdyn_InvokeIntelligenceAction response is null.");

                var result = JsonConvert.DeserializeObject<msdyn_InvokeIntelligenceActionResponse>(resp.Results["Result"] as string);
                
            }
            catch (Exception)
            {

                throw;
            }
          
            

            var response = serviceClient.ExecuteCrmWebRequest(HttpMethod.Post, "msdyn_InvokeIntelligenceAction", sr.ReadToEnd(), null);

            //var newOwner = (EntityReference)resp["AssignedTo"];
        }

        [TestCategory("CustomerServiceCopilot")]
        [TestCategory("msdyn_InvokeIntelligenceAction")]
        [TestMethod]
        public void msdyn_InvokeIntelligenceAction_SDK_CrmServiceClient_SystemUser_EmailAssist_SuggestACall_VerifyResponse()
        {

            CrmServiceClient serviceClient = new CrmServiceClient(_crmServiceClientConnection);

            #region payload
            InvokeIntelligenceActionRequest requestPayload = new InvokeIntelligenceActionRequest();
            requestPayload.ScenarioType = "EmailAssistGPT";
            requestPayload.RequestPayload = new InvokeIntelligenceActionRequestPayload();
            #region Email Assist props
            requestPayload.RequestPayload.agent_name = "System Administrator";
            requestPayload.RequestPayload.agent_text = "";
            requestPayload.RequestPayload.intent = "Suggest a call";
            requestPayload.RequestPayload.organization_id = "e7715f15-b0ab-ee11-be32-002248282e3a";
            requestPayload.Context = new InvokeIntelligenceActionRequestContext();
            requestPayload.Context.scenariorequestid = "b5ae9cc0-befd-8ab1-a80c-f97ad3bf1eeb";
            #endregion
            //requestPayload.RequestPayload.output_mode = "intent";
            requestPayload.RequestPayload.version = "3.0";
            //requestPayload.RequestPayload.content = new InvokeIntelligenceActionRequestPayloadContent();
            //requestPayload.RequestPayload.content.messages = new List<InvokeIntelligenceActionRequestPayloadContentMessage>();
            //requestPayload.RequestPayload.content.messages.Add(new InvokeIntelligenceActionRequestPayloadContentMessage()
            //{
            //    text = "how should i respond to this customers?",
            //    user = "customer",
            //    datetime = DateTime.Now.ToString()
            //});

            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(InvokeIntelligenceActionRequestPayload), new DataContractJsonSerializerSettings()
            {
                UseSimpleDictionaryFormat = true
            });
            ser.WriteObject(stream1, requestPayload.RequestPayload);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);

            #endregion

            var req = new OrganizationRequest("msdyn_InvokeIntelligenceAction")
            {
                ["ScenarioType"] = "IntentIdentification",
                ["RequestPayload"] = sr.ReadToEnd()
            };

            // Web API call
            //var myContent = JsonConvert.SerializeObject(requestPayload);
            try
            {
                var resp = serviceClient.Execute(req);

                Assert.IsNotNull(resp, "msdyn_InvokeIntelligenceAction response is null.");

                var result = JsonConvert.DeserializeObject<msdyn_InvokeIntelligenceActionResponse>(resp.Results["Result"] as string);

            }
            catch (Exception)
            {

                throw;
            }



            var response = serviceClient.ExecuteCrmWebRequest(HttpMethod.Post, "msdyn_InvokeIntelligenceAction", sr.ReadToEnd(), null);

            //var newOwner = (EntityReference)resp["AssignedTo"];
        }

        [TestCategory("CustomerServiceCopilot")]
        [TestCategory("msdyn_InvokeIntelligenceAction")]
        [TestCategory("API")]
        [TestCategory("MSAL")]
        [TestMethod]
        public void msdyn_InvokeIntelligenceAction_API_MSAL_SystemUserToken_WillFail()
        {
            #region auth
            string resource = "";
            var clientId = "";
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