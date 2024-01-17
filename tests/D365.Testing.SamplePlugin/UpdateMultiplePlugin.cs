using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace D365.SamplePlugin
{
    public class UpdateMultiplePlugin : IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context
            IPluginExecutionContext context = (IPluginExecutionContext)
              serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the IOrganizationService instance 
            IOrganizationServiceFactory serviceFactory =
              (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService orgService = serviceFactory.CreateOrganizationService(context.UserId);

            // Obtain the Tracing service reference
            ITracingService tracingService =
              (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Verify input parameters
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity entity)
            {

                // Verify expected entity image from step registration
                if (context.PreEntityImages.TryGetValue("example_preimage", out Entity preImage))
                {

                    bool entityContainsSampleName = entity.Contains("sample_name");
                    bool entityImageContainsSampleName = preImage.Contains("sample_name");
                    bool entityImageContainsSampleDescription = preImage.Contains("sample_description");

                    if (entityContainsSampleName && entityImageContainsSampleName && entityImageContainsSampleDescription)
                    {
                        // Verify that the entity 'sample_name' values are different
                        if (entity["sample_name"] != preImage["sample_name"])
                        {
                            string newName = (string)entity["sample_name"];
                            string oldName = (string)preImage["sample_name"];
                            string message = $"\\r\\n - 'sample_name' changed from '{oldName}' to '{newName}'.";

                            // If the 'sample_description' is included in the update, do not overwrite it, just append to it.
                            if (entity.Contains("sample_description"))
                            {

                                entity["sample_description"] = entity["sample_description"] += message;

                            }
                            else // The sample description is not included in the update, overwrite with current value + addition.
                            {
                                entity["sample_description"] = preImage["sample_description"] += message;
                            }

                            // Success:
                            tracingService.Trace($"Appended to 'sample_description': \"{message}\" ");
                        }
                        else
                        {
                            tracingService.Trace($"Expected entity and preImage 'sample_name' values to be different. Both are {entity["sample_name"]}");
                        }
                    }
                    else
                    {
                        if (!entityContainsSampleName)
                            tracingService.Trace("Expected entity sample_name attribute not found.");
                        if (!entityImageContainsSampleName)
                            tracingService.Trace("Expected preImage entity sample_name attribute not found.");
                        if (!entityImageContainsSampleDescription)
                            tracingService.Trace("Expected preImage entity sample_description attribute not found.");
                    }
                }
                else
                {
                    tracingService.Trace($"Expected PreEntityImage: 'example_preimage' not found.");
                }
            }
            else
            {
                if (!context.InputParameters.Contains("Target"))
                    tracingService.Trace($"Expected InputParameter: 'Target' not found.");
                if (!(context.InputParameters["Target"] is Entity))
                    tracingService.Trace($"Expected InputParameter: 'Target' is not Entity.");
            }
        }
    }
}
