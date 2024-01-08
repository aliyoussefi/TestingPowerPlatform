using System;
using System.Net.Http;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using OrderProductRetrievalPlugin;

namespace WebServiceCallPlugin
{
    public class ExternalWebServicePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the organization service reference.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // Make the web service call.
                MakeWebServiceCall();
                //EntityCollection orderProducts = service.RetrieveMultiple(
                //            new FetchExpression(
                //                $@"
                //                <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                //                    <entity name='salesorderdetail'>
                //                        <attribute name='productid' />
                //                        <attribute name='quantity' />
                //                        <filter type='and'>
                //                            <condition attribute='salesorderid' operator='eq' value='{order.Id}' />
                //                        </filter>
                //                    </entity>
                //                </fetch>"
                //            )
                //        );
                //RetrieveOrderProducts retrieveOrderProducts = new RetrieveOrderProducts().ProcessOrderProducts(orderProducts);
                // Optionally, you can perform other operations or logic after the web service call.
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"An error occurred: {ex.Message}", ex);
            }
        }

        public HttpResponseMessage MakeWebServiceCall()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                // Define the endpoint URL.
                string endpointUrl = "https://api.example.com/resource/getorder";

                // Optionally, set headers or other configurations as needed.
                // httpClient.DefaultRequestHeaders.Add("HeaderName", "HeaderValue");

                try
                {
                    // Make the GET request (or POST, PUT, etc. based on your needs).
                    HttpResponseMessage response = httpClient.GetAsync(endpointUrl).Result;

                    // Ensure the response is successful.
                    response.EnsureSuccessStatusCode();

                    // Optionally, read and process the response content.
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    // Process the responseBody as needed.
                    // ...
                    return response;
                }
                catch (HttpRequestException e)
                {
                    throw new Exception($"Error making web service call: {e.Message}", e);
                }
            }
        }
    }
}
