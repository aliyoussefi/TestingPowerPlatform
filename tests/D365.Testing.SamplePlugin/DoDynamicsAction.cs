using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace D365.Testing.SamplePlugin
{
    public class DoDynamicsAction : IPlugin {

        #region prop
        private readonly string _unsecureString;
        private readonly string _secureString;
        private string _instrumentationKey;
        #endregion
        #region ctor
        public DoDynamicsAction(string unsecureConfig, string secureConfig)
        {
            _unsecureString = unsecureConfig;
            _secureString = secureConfig;
        }
        #endregion
        #region Helpers


        private string GetValueNode(XmlDocument doc, string key)
        {
            XmlNode node = doc.SelectSingleNode(String.Format("Settings/setting[@name='{0}']", key));

            if (node != null)
            {
                return node.SelectSingleNode("value").InnerText;
            }
            return string.Empty;
        }
        #endregion

        public void Execute(IServiceProvider serviceProvider) {
            //https://msdn.microsoft.com/en-us/library/gg509027.aspx
            //When you use the Update method or UpdateRequest message, do not set the OwnerId attribute on a record unless the owner has actually changed.
            //When you set this attribute, the changes often cascade to related entities, which increases the time that is required for the update operation.
            //Extract the tracing service for use in debugging sandboxed plug-ins.
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace("Starting ShowTargetInputParameterAttributes at " + DateTime.Now.ToString());
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));
            tracingService.Trace("BusinessUnitId: " + context.BusinessUnitId.ToString());
            tracingService.Trace("CorrelationId: " + context.CorrelationId.ToString());
            tracingService.Trace("Depth: " + context.Depth.ToString());
            tracingService.Trace("InitiatingUserId: " + context.InitiatingUserId.ToString());
            tracingService.Trace("InputParameters.Count: " + context.InputParameters.Count.ToString());
            if (context.InputParameters.Count > 0) {
                tracingService.Trace("Begin Write the InputParameters");
                //write it!
                foreach (var x in context.InputParameters) {
                    WriteTargetAttribute(x, tracingService);
                }
                tracingService.Trace("End Write the InputParameters");
            }
            tracingService.Trace("IsExecutingOffline: " + context.IsExecutingOffline.ToString());
            tracingService.Trace("IsInTransaction: " + context.IsInTransaction.ToString());
            tracingService.Trace("IsOfflinePlayback: " + context.IsOfflinePlayback.ToString());
            tracingService.Trace("IsolationMode: " + context.IsolationMode.ToString());
            tracingService.Trace("MessageName: " + context.MessageName.ToString());
            tracingService.Trace("Mode: " + context.Mode.ToString());
            tracingService.Trace("OperationCreatedOn: " + context.OperationCreatedOn.ToString());
            tracingService.Trace("OperationId: " + context.OperationId.ToString());
            tracingService.Trace("OrganizationId: " + context.OrganizationId.ToString());
            tracingService.Trace("OrganizationName: " + context.OrganizationName.ToString());
            tracingService.Trace("OutputParameters.Count: " + context.OutputParameters.Count.ToString());
            if (context.OutputParameters.Count > 0) {
                tracingService.Trace("Begin Write the OutputParameters");
                //write it!
                foreach (var x in context.OutputParameters) {
                    WriteTargetAttribute(x, tracingService);
                }
                tracingService.Trace("End Write the OutputParameters");
            }
            tracingService.Trace("OwningExtension: " + context.OwningExtension.ToString());
            //tracingService.Trace("ParentContext: " + context.ParentContext.ToString());
            tracingService.Trace("PostEntityImages.Count: " + context.PostEntityImages.Count.ToString());
            if (context.PostEntityImages.Count > 0) {
                foreach (KeyValuePair<string, Entity> preImage in context.PostEntityImages) {

                    // Obtain the target entity from the input parameters.
                    Entity entity = (Entity)preImage.Value;
                    //entity.Attributes["ownerid"] = new EntityReference("systemuser" , Guid.NewGuid());
                    foreach (KeyValuePair<string, object> attr in entity.Attributes) {
                        WriteTargetAttribute(attr, tracingService);

                    }


                }
            }
            tracingService.Trace("PreEntityImages.Count: " + context.PreEntityImages.Count.ToString());
            if (context.PreEntityImages.Count > 0) {
                foreach (KeyValuePair<string, Entity> preImage in context.PreEntityImages) {

                    // Obtain the target entity from the input parameters.
                    Entity entity = (Entity)preImage.Value;
                    //entity.Attributes["ownerid"] = new EntityReference("systemuser" , Guid.NewGuid());
                    foreach (KeyValuePair<string, object> attr in entity.Attributes) {
                        WriteTargetAttribute(attr, tracingService);

                    }


                }
            }

            tracingService.Trace("PrimaryEntityId: " + context.PrimaryEntityId.ToString());
            tracingService.Trace("PrimaryEntityName: " + context.PrimaryEntityName.ToString());

            tracingService.Trace("RequestId: " + context.RequestId.ToString());
            tracingService.Trace("SecondaryEntityName: " + context.SecondaryEntityName.ToString());
            tracingService.Trace("SharedVariables.Count: " + context.SharedVariables.Count.ToString());
            if (context.SharedVariables.Count > 0) {
                tracingService.Trace("Begin Write the SharedVariables");
                //write it!
                foreach (var x in context.SharedVariables) {
                    WriteTargetAttribute(x, tracingService);
                }
                tracingService.Trace("End Write the SharedVariables");
            }
            tracingService.Trace("Stage: " + context.Stage.ToString());
            tracingService.Trace("UserId: " + context.UserId.ToString());
            if (context.MessageName == "QualifyLead") {
                tracingService.Trace("I am in a plugin that has a QualifyLead message.");
            }
            // The InputParameters collection contains all the data passed in the message request.
            if (context.InputParameters.Contains("Target") &&
            context.InputParameters["Target"] is Entity) {
                // Obtain the target entity from the input parameters.
                Entity entity = (Entity)context.InputParameters["Target"];
                //entity.Attributes["ownerid"] = new EntityReference("systemuser" , Guid.NewGuid());
                foreach (KeyValuePair<string, object> attr in entity.Attributes) {
                    WriteTargetAttribute(attr, tracingService);

                }
                //if (context.MessageName == "QualifyLead")
                //{
                //    tracingService.Trace("I am in a plugin that has a QualifyLead message.");
                //}
                //if ((context.MessageName == "Update" || context.MessageName == "QualifyLead") && context.Stage != 10)
                //{
                //    tracingService.Trace("I am in a plugin that has an Update Message and is not of Stage 10.");
                //    tracingService.Trace("Set the serviceFactory.");
                //    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                //    tracingService.Trace("Set the organizationService.");
                //    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                //    tracingService.Trace("Get the regarding object entity.");

                //    foreach (KeyValuePair<string, object> attr in entity.Attributes)
                //    {
                //        WriteTargetAttribute(attr, tracingService);

                //    }
                //    tracingService.Trace("Update Target.");
                //    service.Update(entity);
                //}
            }


            tracingService.Trace("Custom for Adobe, this is not in CRM yet.");
            var xx = "test";
            if (xx == "test") {
                tracingService.Trace("testing new string");
            }
            tracingService.Trace("Ending ShowTargetInputParameterAttributes at " + DateTime.Now.ToString());
        }

        private void WriteTargetAttribute(KeyValuePair<string, object> attr, ITracingService tracingService) {
            if ((attr.Value != null)) {
                switch (attr.Value.GetType().Name) {
                    case "EntityReference":
                        tracingService.Trace("Entity attribute: " + attr.Key + " Value: " + ((EntityReference)attr.Value).Name);
                        break;
                    case "Money":
                        tracingService.Trace("Entity attribute: " + attr.Key + " Value: " + ((Money)attr.Value).Value);
                        break;
                    case "OptionSetValue":
                        tracingService.Trace("Entity attribute: " + attr.Key + " Value: " + ((OptionSetValue)attr.Value).Value);
                        break;
                    case "System.Collections.Generic.Dictionary`2[System.String,System.String]":
                        tracingService.Trace("Entity attribute: " + attr.Key + " Value: " + ((Dictionary<string, string>)attr.Value).Count.ToString());
                        break;
                    case "":
                        tracingService.Trace("Entity attribute: " + attr.Key + " Value: Unknown");
                        break;
                    case "System.Byte[]":
                        tracingService.Trace("Entity attribute: " + attr.Key);
                        tracingService.Trace("Entity attribute: " + attr.Key + " Value: " + Convert.ToBase64String(((byte[])attr.Value)));
                        break;
                    default:
                        tracingService.Trace("Entity attribute: " + attr.Key + " Value: " + attr.Value.ToString());
                        break;

                }
            }


        }

    }
}
