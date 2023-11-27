using FakeXrmEasy;
using FakeXrmEasy.Abstractions.Plugins.Enums;
using FakeXrmEasy.Pipeline;
using FakeXrmEasy.Plugins;
using FakeXrmEasy.Plugins.PluginSteps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;
using System.Windows.Controls;

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
        public void TestMultipleRetrieveCalls_Update_PostOperation() {
            //Arrange
            var pluginContext = _context.GetDefaultPluginContext();

            pluginContext.MessageName = "Update";
            pluginContext.Stage = 40;
            
            string unsecureConfig = "";
            string secureConfig = "";
            try{
                var result = _context.ExecutePluginWithConfigurations<Dynamics365.Monitoring.Plugins.MultipleRetrieveCalls>(pluginContext, unsecureConfig, secureConfig);
            }
            catch (Exception ex){
                throw ex;
            }
            //Assert.IsTrue(((int)result["rtnInteger"]).Equals(5));
        }
        [TestMethod]
        public void TestResourceLockPlugin_Update_PostOperation()
        {
            //Arrange
            var pluginContext = _context.GetDefaultPluginContext();

            pluginContext.MessageName = "Update";
            pluginContext.Stage = 40;
            
            Entity accountEntity = new Entity("ali_monitoringperformance");
            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", accountEntity);

            _context.RegisterPluginStep<Dynamics365.Monitoring.Plugins.ResourceLockPlugin>(new PluginStepDefinition()
            {
                MessageName = "Update",
                EntityLogicalName = "ali_monitoringperformance",
                Stage = ProcessingStepStage.Preoperation
            });

            string unsecureConfig = "";
            string secureConfig = "";

            pluginContext.InputParameters = inputParameters;

            try
            {
                var result = _context.ExecutePluginWith<Dynamics365.Monitoring.Plugins.ResourceLockPlugin>(pluginContext);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //Assert.IsTrue(((int)result["rtnInteger"]).Equals(5));
        }

        [TestMethod]
        public void TestSleepyPlugin_Update_PostOperation()
        {
            //Arrange
            var pluginContext = _context.GetDefaultPluginContext();

            pluginContext.MessageName = "Update";
            pluginContext.Stage = 40;
            Entity accountEntity = new Entity("ali_monitoringperformance");
            pluginContext.InputParameters = new ParameterCollection();
            pluginContext.InputParameters.Add(new KeyValuePair<string, object>("Target", accountEntity));

            string unsecureConfig = "";
            string secureConfig = "";
            try
            {
                var result = _context.ExecutePluginWithConfigurations<Dynamics365.Monitoring.Plugins.SleepyPlugin>(pluginContext, unsecureConfig, secureConfig);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //Assert.IsTrue(((int)result["rtnInteger"]).Equals(5));
        }

        [TestMethod]
        public void TestUpdateRecursivePlugin_Update_PostOperation_Success()
        {
            //Arrange
            var pluginContext = _context.GetDefaultPluginContext();

            pluginContext.MessageName = "Update";
            pluginContext.Stage = 40;
            Entity accountEntity = new Entity("ali_monitoringperformance");
            accountEntity.Attributes = new AttributeCollection();
            accountEntity.Attributes.Add(new KeyValuePair<string, object>("ayw_instrumentationkey", "12345"));
            //((EntityReference)accountEntity.Attributes["parentaccountid"]).Id = null;

            pluginContext.InputParameters = new ParameterCollection();
            pluginContext.InputParameters.Add(new KeyValuePair<string, object>("Target", accountEntity));
            string unsecureConfig = "";
            string secureConfig = "";
            try
            {
                var result = _context.ExecutePluginWithConfigurations<Dynamics365.Monitoring.Plugins.UpdateRecursive>(pluginContext, unsecureConfig, secureConfig);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //Assert.IsTrue(((int)result["rtnInteger"]).Equals(5));
        }

        [TestMethod]
        public void TestUpdateRecursivePlugin_Update_PostOperation_Failure()
        {
            //Arrange
            var pluginContext = _context.GetDefaultPluginContext();

            pluginContext.MessageName = "Update";
            pluginContext.Stage = 40;
            Entity accountEntity = new Entity();
            accountEntity.Attributes = new AttributeCollection();
            accountEntity.Attributes.Add(new KeyValuePair<string, object>("branch", "Some Branch"));
            accountEntity.Attributes.Add(new KeyValuePair<string, object>("parentaccountid", new EntityReference("parentaccount", null)));
            //((EntityReference)accountEntity.Attributes["parentaccountid"]).Id = null;

            pluginContext.InputParameters = new ParameterCollection();
            pluginContext.InputParameters.Add(new KeyValuePair<string, object>("Target", accountEntity));
            string unsecureConfig = "";
            string secureConfig = "";
            try
            {
                var result = _context.ExecutePluginWithConfigurations<Dynamics365.Monitoring.Plugins.UpdateRecursive>(pluginContext, unsecureConfig, secureConfig);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //Assert.IsTrue(((int)result["rtnInteger"]).Equals(5));
        }
    }
}
