using System;
using System.Net.Http;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Identity.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace D365.SamplePlugin
{
    public class ExternalWebServicePlugin : IPlugin
    {
        public interface IWebService
        {
             string MakeCall();
        }
        //public class ExternalWebService : IWebService
        //{
        //    public string MakeCall()
        //    {
        //        return string.Empty;
        //        //throw new NotImplementedException();
        //    }
        //}

        //public IWebService MakeCall()
        //{
        //    return new ExternalWebService();
        //    // return an instance to my external service
        //}

        public IWebService myService;

        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the organization service reference.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                //IWebService myExternalService = new ExternalWebService();
                //// Make the web service call.
                ////MakeWebServiceCall();
                //var response2 = MakeCall();
                //var response = myExternalService.MakeCall();

                var response = myService.MakeCall();

                AddListMembersListRequest exampleMockedCall = new AddListMembersListRequest();
                exampleMockedCall.MemberIds = new Guid[] {Guid.NewGuid() };
                exampleMockedCall.ListId = Guid.NewGuid();
                service.Execute(exampleMockedCall);
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
