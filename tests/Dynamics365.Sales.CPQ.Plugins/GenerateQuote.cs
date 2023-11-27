using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.Sales.CPQ.Plugins
{
    public class GenerateQuote : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            ILogger logger = (ILogger)serviceProvider.GetService(typeof(ILogger));

            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.
                Entity entity = (Entity)context.InputParameters["Target"];
                //entity.Attributes["ownerid"] = new EntityReference("systemuser" , Guid.NewGuid());
                //foreach (KeyValuePair<string, object> attr in entity.Attributes)
                //{
                //    WriteTargetAttribute(attr, tracingService);

                //}
                service.Update(entity);
            }

            logger.LogInformation("GenerateQuote from CPQ");
        }
    }
}
