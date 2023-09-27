using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using D365.Testing.FakeXrmEasy.TestConfig;
using FakeXrmEasy;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace D365.Testing.FakeXrmEasy {
    [TestClass]
    public class FakeXrmEasyUnitTests {

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
        public void TestMethodWithLateBound() {

            //
            // TODO: Add test logic here
            //

            var fakedContext = new XrmFakedContext();
            var fakedExecutionContext = new XrmFakedPluginExecutionContext();
            var wfContext = fakedContext.GetDefaultPluginContext();
            wfContext.MessageName = "Update";
            wfContext.Stage = 40;
            Debug.WriteLine("This is a test line.");
            Entity accountEntity = new Entity();
            accountEntity.Attributes = new AttributeCollection();
            accountEntity.Attributes.Add(new KeyValuePair<string, object>("branch", "Some Branch"));
            accountEntity.Attributes.Add(new KeyValuePair<string, object>("parentaccountid", new EntityReference("parentaccount", null)));
            //((EntityReference)accountEntity.Attributes["parentaccountid"]).Id = null;

            wfContext.InputParameters = new ParameterCollection();
            wfContext.InputParameters.Add(new KeyValuePair<string, object>("Target", accountEntity));
            Debug.WriteLine("This is a test line 2.");
            //PreImage
            Entity PreImage = new Entity();
            PreImage.Attributes = new AttributeCollection();
            PreImage.Attributes.Add(new KeyValuePair<string, object>("branch", "Some Branch"));
            PreImage.Attributes.Add(new KeyValuePair<string, object>("parentaccountid", new EntityReference("parentaccount", Guid.NewGuid())));

            wfContext.PreEntityImages = new EntityImageCollection();
            wfContext.PreEntityImages.Add(new KeyValuePair<string, Entity>("PreBusinessEntity", PreImage));

            string unsecureConfig = "";
            string secureConfig = "";

            Debug.WriteLine("This is a test line 3.");
            try
            {
                var result = fakedContext.ExecutePluginWithConfigurations<D365.Testing.SamplePlugin.DoDynamicsAction>(wfContext, unsecureConfig, secureConfig);
                Debug.WriteLine("This is a test line 4.");
            }
            catch (Exception ex)
            {

                throw ex;
            }
            

            //Assert.IsTrue(((int)result["rtnInteger"]).Equals(5));
        }
        [TestMethod]
        public void TestMethodWithEarlyBound()
        {

            //
            // TODO: Add test logic here
            //

            var fakedContext = new XrmFakedContext();
            var wfContext = fakedContext.GetDefaultPluginContext();
            wfContext.MessageName = "Update";
            wfContext.Stage = 40;
            Debug.WriteLine("This is a test line.");
            Account accountEntity = new TestConfig.Account();
            accountEntity.Attributes = new AttributeCollection();
            accountEntity.Attributes.Add(new KeyValuePair<string, object>(accountEntity.branch, "Some Branch"));
            //accountEntity.Attributes.Add(new KeyValuePair<string, object>(accountEntity.parentaccount., new EntityReference("parentaccount", null)));
            //((EntityReference)accountEntity.Attributes["parentaccountid"]).Id = null;

            wfContext.InputParameters = new ParameterCollection();
            wfContext.InputParameters.Add(new KeyValuePair<string, object>("Target", accountEntity));
            Debug.WriteLine("This is a test line 2.");
            //PreImage
            Entity PreImage = new Entity();
            PreImage.Attributes = new AttributeCollection();
            PreImage.Attributes.Add(new KeyValuePair<string, object>("branch", "Some Branch"));
            PreImage.Attributes.Add(new KeyValuePair<string, object>("parentaccountid", new EntityReference("parentaccount", Guid.NewGuid())));

            wfContext.PreEntityImages = new EntityImageCollection();
            wfContext.PreEntityImages.Add(new KeyValuePair<string, Entity>("PreBusinessEntity", PreImage));
            Debug.WriteLine("This is a test line 3.");
            try
            {
                string unsecureConfig = "";
                string secureConfig = "";
                var result = fakedContext.ExecutePluginWithConfigurations<D365.Testing.SamplePlugin.DoDynamicsAction>(wfContext, unsecureConfig, secureConfig);
                Debug.WriteLine("This is a test line 4.");

            }
            catch (Exception ex)
            {

                throw ex;
            }


            //Assert.IsTrue(((int)result["rtnInteger"]).Equals(5));
        }
    }
}
