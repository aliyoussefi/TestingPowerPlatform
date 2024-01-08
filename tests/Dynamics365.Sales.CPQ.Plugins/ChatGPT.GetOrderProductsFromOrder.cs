using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace OrderProductRetrievalPlugin
{
    public class RetrieveOrderProducts : IPlugin
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
                // Check if the plugin was triggered by an order creation.
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity order = (Entity)context.InputParameters["Target"];

                    // Check if the entity is of type "salesorder".
                    if (order.LogicalName == "salesorder")
                    {
                        // Retrieve order products related to the order.
                        EntityCollection orderProducts = service.RetrieveMultiple(
                            new FetchExpression(
                                $@"
                                <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='salesorderdetail'>
                                        <attribute name='productid' />
                                        <attribute name='quantity' />
                                        <filter type='and'>
                                            <condition attribute='salesorderid' operator='eq' value='{order.Id}' />
                                        </filter>
                                    </entity>
                                </fetch>"
                            )
                        );

                        ProcessOrderProducts(orderProducts);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"An error occurred: {ex.Message}", ex);
            }
        }

        internal void ProcessOrderProducts(EntityCollection orderProducts)
        {
            // Handle retrieved order products as needed.
            foreach (var orderProduct in orderProducts.Entities)
            {
                var productId = ((EntityReference)orderProduct["productid"]).Id;
                var quantity = orderProduct.GetAttributeValue<decimal>("quantity");

                // Do something with productId and quantity...
            }
        }
    }
}
