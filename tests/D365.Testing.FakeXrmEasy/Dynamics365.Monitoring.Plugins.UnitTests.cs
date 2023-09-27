using FakeXrmEasy;
using FakeXrmEasy.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;

namespace D365.Testing
{


    [TestClass]
    public class Dynamics365MonitoringPluginsUnitTests : FakeXrmEasyTestsBase {

        public string BuildUnsecureConfig()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<config><settings>");
            sb.Append("<setting>");
            sb.Append(String.Format("<name={0}>", ""));
            sb.Append(String.Format("<order={0}>", ""));
            sb.Append(String.Format("<type={0}>", ""));
            sb.Append("</setting>");
            sb.Append("</settings></config>");

            return sb.ToString();
        }

        [TestMethod]
        public void TestMultipleRetrieveCalls() {

            //
            // TODO: Add test logic here
            //

            //Arrange
            var pluginContext = _context.GetDefaultPluginContext();

            pluginContext.MessageName = "Update";
            pluginContext.Stage = 40;
            Debug.WriteLine("This is a test line.");
            Entity accountEntity = new Entity();
            accountEntity.Attributes = new AttributeCollection();
            accountEntity.Attributes.Add(new KeyValuePair<string, object>("branch", "Some Branch"));
            accountEntity.Attributes.Add(new KeyValuePair<string, object>("parentaccountid", new EntityReference("parentaccount", null)));
            //((EntityReference)accountEntity.Attributes["parentaccountid"]).Id = null;

            pluginContext.InputParameters = new ParameterCollection();
            pluginContext.InputParameters.Add(new KeyValuePair<string, object>("Target", accountEntity));
            Debug.WriteLine("This is a test line 2.");
            //PreImage
            Entity PreImage = new Entity();
            PreImage.Attributes = new AttributeCollection();
            PreImage.Attributes.Add(new KeyValuePair<string, object>("branch", "Some Branch"));
            PreImage.Attributes.Add(new KeyValuePair<string, object>("parentaccountid", new EntityReference("parentaccount", Guid.NewGuid())));

            pluginContext.PreEntityImages = new EntityImageCollection();
            pluginContext.PreEntityImages.Add(new KeyValuePair<string, Entity>("PreBusinessEntity", PreImage));

            string unsecureConfig = "";
            string secureConfig = "";

            Debug.WriteLine("This is a test line 3.");
            try
            {
                var result = _context.ExecutePluginWithConfigurations<Dynamics365.Monitoring.Plugins.MultipleRetrieveCalls>(pluginContext, unsecureConfig, secureConfig);
                Debug.WriteLine("This is a test line 4.");
            }
            catch (Exception ex)
            {

                throw ex;
            }
            

            //Assert.IsTrue(((int)result["rtnInteger"]).Equals(5));
        }
        [TestMethod]
        public void TestResourceLockPlugin()
        {

           

            //Assert.IsTrue(((int)result["rtnInteger"]).Equals(5));
        }
    }
}
