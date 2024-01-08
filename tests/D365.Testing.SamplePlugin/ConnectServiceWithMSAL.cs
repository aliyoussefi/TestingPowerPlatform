using Microsoft.Identity.Client;
using Microsoft.Xrm.Sdk;
using System;

namespace D365.SamplePlugin
{
    public class ConnectServiceWithMSAL : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            string clientId = "test";
            string clientSecret = "test";
            string tenantId = "";
            string[] scopes = { "" }; //no where is this
            ITracingService tracingService =
                        (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace("Starting IConfidentialClientApplication at " + DateTime.Now.ToString());
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
                .WithTenantId(tenantId)
                .WithClientId(clientId)
                .Build();

            Microsoft.Identity.Client.AuthenticationResult result = null;
            try
            {
                tracingService.Trace("Starting AcquireTokenForClient at " + DateTime.Now.ToString());
                result = app.AcquireTokenForClient(scopes).ExecuteAsync().GetAwaiter().GetResult();
            }
            catch (MsalUiRequiredException e)
            {
                //Console.WriteLine(e.Message);
                throw e;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }
    }
}
