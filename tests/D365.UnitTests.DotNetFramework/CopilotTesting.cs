using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;

namespace D365.UnitTests.Copilot
{
    [TestClass]
    public class CopilotTesting
    {
        private static string _conn;
        private static string _clientId;
        private static string _clientSecret;
        private static string _tenantId;
        private static string _orgUrl;
        private static TestContext _testContext;

        [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
        public static void Initialize(TestContext TestContext)
        {
            _testContext = TestContext;
            _conn = _testContext.Properties["Connection"].ToString();

            _clientId = _testContext.Properties["clientId"].ToString();
            _clientSecret = _testContext.Properties["clientSecret"].ToString();
            _tenantId = _testContext.Properties["tenantId"].ToString();
            _orgUrl = _testContext.Properties["orgUrl"].ToString();
        }

        #region Invoke Intellience Action Classes
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
        }

        public class InvokeIntelligenceActionRequestContext
        {
            public string scenariorequestid { get; set; }
            public string regardingobjectid { get; set; }
            public bool verifyfullaccess { get; set; }
        }
        #endregion

        #region InvokeCopilotAPI
        public class InvokeCopilotAPIRequestPayload
        {
            public InvokeCopilotAPIRequestPayloadMessageData messagedata { get; set; }
            public string orgUrl { get; set; }
            public string orgId { get; set; }
            public string requestId { get; set; }
            public string category { get; set; }
            public InvokeCopilotAPIRequestPayloadKnobsConfigurations knobsConfigurations { get; set; }
        }
        public class InvokeCopilotAPIRequestPayloadMessageData
        {
            public string body { get; set; }
        }
        public class InvokeCopilotAPIRequestPayloadKnobsConfigurations
        {
            public string length { get; set; }
            public string tone { get; set; }
        }
        #endregion

        #region Authentication
        private string ConnectToDynamics(S2SAuthenticationSettings authenticationSettings)
        {
            Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential clientcred = new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential(authenticationSettings.clientId, authenticationSettings.clientSecret);
            AuthenticationContext authenticationContext = new AuthenticationContext(authenticationSettings.aadInstance + authenticationSettings.tenantID);
            var authenticationResult = authenticationContext.AcquireTokenAsync(authenticationSettings.organizationUrl, clientcred).Result;
            return authenticationResult.AccessToken;

        }

        public class S2SAuthenticationSettings
        {
            public string organizationUrl;
            public string clientId;
            public string clientSecret;
            public string aadInstance = "https://login.microsoftonline.com/";
            public string tenantID;
        }
        #endregion


      //  [TestCategory("Dynamics365Copilot")]
      //  [TestCategory("msdyn_InvokeCopilotAPIRequest")]
      //  [TestCategory("API")]
      //  [TestMethod]
      //  public void RichTextEdtiorResponseValidator_API_SystemUserToken_WillFail()
      //  {

      //      string resource = "";
      //      var clientId = "";
      //      var redirectUri = "http://localhost"; // Loopback for the interactive login.

      //      // MSAL authentication
      //      var authBuilder = PublicClientApplicationBuilder.Create(clientId)
      //          .WithAuthority(AadAuthorityAudience.AzureAdMultipleOrgs)
      //          .WithRedirectUri(redirectUri)
      //          .Build();
      //      var scope = resource + "/user_impersonation";
      //      string[] scopes = { scope };

      //      Microsoft.Identity.Client.AuthenticationResult token =
      //          authBuilder.AcquireTokenInteractive(scopes).ExecuteAsync().Result;

      //      // Set up the HTTP client
      //      var client = new HttpClient
      //      {
      //          BaseAddress = new Uri(resource + "/api/data/v9.2/"),
      //          Timeout = new TimeSpan(0, 2, 0)  // Standard two minute timeout.
      //      };

      //      HttpRequestHeaders headers = client.DefaultRequestHeaders;
      //      headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
      //      headers.Add("OData-MaxVersion", "4.0");
      //      headers.Add("OData-Version", "4.0");
      //      headers.Accept.Add(
      //          new MediaTypeWithQualityHeaderValue("application/json"));

      //      // Web API call
      //      var response = client.GetAsync("WhoAmI").Result;

      //      //Microsoft.PowerPlatform.Dataverse.Client.ServiceClient serviceClient = new Microsoft.PowerPlatform.Dataverse.Client.ServiceClient(_conn);
      //      msdyn_InvokeCopilotAPIRequest copilotReq = new msdyn_InvokeCopilotAPIRequest();
      //      copilotReq.msdyn_APIName = "EmailAssist";
      //      InvokeCopilotAPIRequestPayload payload = new InvokeCopilotAPIRequestPayload();
      //      payload.messagedata = new InvokeCopilotAPIRequestPayloadMessageData();
      //      payload.messagedata.body = "blah blah";
      //      payload.category = "RefineDraft";
      //      payload.orgId = "e7715f15-b0ab-ee11-be32-002248282e3a";
      //      payload.orgUrl = @"https://orgb66e00bc.crm.dynamics.com";
      //      MemoryStream stream1 = new MemoryStream();
      //      DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(InvokeCopilotAPIRequestPayload), new DataContractJsonSerializerSettings()
      //      {
      //          UseSimpleDictionaryFormat = true
      //      });
      //      ser.WriteObject(stream1, payload);
      //      stream1.Position = 0;
      //      StreamReader sr = new StreamReader(stream1);
      //      payload.requestId = "";
      //      payload.knobsConfigurations = new InvokeCopilotAPIRequestPayloadKnobsConfigurations()
      //      {
      //          length = "relative",
      //          tone = "standard"
      //      };
      //      copilotReq.msdyn_payload = sr.ReadToEnd();

      //      object data = new
      //      {
      //          msdyn_APIName = "EmailAssist",
      //          msdyn_payload = copilotReq.msdyn_payload
      //      };

      //      // Set up the HTTP client
      //      //var client = new HttpClient
      //      //{
      //      //    BaseAddress = new Uri(_orgUrl + "api/data/v9.2/"),
      //      //    Timeout = new TimeSpan(0, 2, 0),  // Standard two minute timeout.

      //      //};

      //      //HttpRequestHeaders headers = client.DefaultRequestHeaders;
      //      headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
      //      headers.Add("OData-MaxVersion", "4.0");
      //      headers.Add("OData-Version", "4.0");
      //      //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

      //      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://orgb66e00bc.api.crm.dynamics.com/api/data/v9.2/msdyn_InvokeCopilotAPI");


      //      headers.Accept.Add(
      //          new MediaTypeWithQualityHeaderValue("application/json"));

      //      // Web API call
      //      var myContent = JsonConvert.SerializeObject(data);
      //      var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
      //      var byteContent = new ByteArrayContent(buffer);
      //      request.Content = new StringContent(myContent,
      //                              Encoding.UTF8,
      //                              "application/json");//CONTENT-TYPE header

      //      client.SendAsync(request)
      //.ContinueWith(responseTask =>
      //{
      //    Console.WriteLine("Response: {0}", responseTask.Result);
      //}).GetAwaiter().GetResult();
      //  }

      //  [TestCategory("Dynamics365Copilot")]
      //  [TestCategory("msdyn_InvokeCopilotAPIRequest")]
      //  [TestCategory("API")]
      //  [TestMethod]
      //  public void RichTextEdtiorResponseValidator_API_WillFail()
      //  {

      //      string token = ConnectToDynamics(new S2SAuthenticationSettings()
      //      {
      //          clientId = _clientId,
      //          clientSecret = _clientSecret,
      //          tenantID = _tenantId,
      //          organizationUrl = _orgUrl
      //      });


      //      //Microsoft.PowerPlatform.Dataverse.Client.ServiceClient serviceClient = new Microsoft.PowerPlatform.Dataverse.Client.ServiceClient(_conn);
      //      msdyn_InvokeCopilotAPIRequest copilotReq = new msdyn_InvokeCopilotAPIRequest();
      //      copilotReq.msdyn_APIName = "EmailAssist";
      //      InvokeCopilotAPIRequestPayload payload = new InvokeCopilotAPIRequestPayload();
      //      payload.messagedata = new InvokeCopilotAPIRequestPayloadMessageData();
      //      payload.messagedata.body = "blah blah";
      //      payload.category = "RefineDraft";
      //      payload.orgId = "b627281c-41e8-ed11-a80b-000d3a323627";
      //      payload.orgUrl = @"https://orgb24a69ed.crm.dynamics.com";
      //      MemoryStream stream1 = new MemoryStream();
      //      DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(InvokeCopilotAPIRequestPayload), new DataContractJsonSerializerSettings()
      //      {
      //          UseSimpleDictionaryFormat = true
      //      });
      //      ser.WriteObject(stream1, payload);
      //      stream1.Position = 0;
      //      StreamReader sr = new StreamReader(stream1);
      //      payload.requestId = "";
      //      payload.knobsConfigurations = new InvokeCopilotAPIRequestPayloadKnobsConfigurations()
      //      {
      //          length = "relative",
      //          tone = "standard"
      //      };
      //      copilotReq.msdyn_payload = sr.ReadToEnd();

      //      object data = new
      //      {
      //          msdyn_APIName = "EmailAssist",
      //          msdyn_payload = copilotReq.msdyn_payload
      //      };

      //      // Set up the HTTP client
      //      var client = new HttpClient
      //      {
      //          BaseAddress = new Uri(_orgUrl + "api/data/v9.2/"),
      //          Timeout = new TimeSpan(0, 2, 0),  // Standard two minute timeout.

      //      };

      //      HttpRequestHeaders headers = client.DefaultRequestHeaders;
      //      headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
      //      headers.Add("OData-MaxVersion", "4.0");
      //      headers.Add("OData-Version", "4.0");
      //      //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

      //      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _orgUrl + "api/data/v9.2/msdyn_InvokeCopilotAPI");


      //      headers.Accept.Add(
      //          new MediaTypeWithQualityHeaderValue("application/json"));

      //      // Web API call
      //      var myContent = JsonConvert.SerializeObject(data);
      //      var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
      //      var byteContent = new ByteArrayContent(buffer);
      //      request.Content = new StringContent(myContent,
      //                              Encoding.UTF8,
      //                              "application/json");//CONTENT-TYPE header

      //      client.SendAsync(request)
      //.ContinueWith(responseTask =>
      //{
      //    Console.WriteLine("Response: {0}", responseTask.Result);
      //}).GetAwaiter().GetResult();
      //      //var response = client.PostAsync("msdyn_InvokeCopilotAPI", byteContent).Result;

      //      //var newOwner = (EntityReference)resp["AssignedTo"];
      //  }
    }
}