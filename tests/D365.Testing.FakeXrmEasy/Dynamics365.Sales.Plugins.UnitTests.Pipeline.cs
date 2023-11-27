using D365.Testing.enums;
using FakeXrmEasy;
using FakeXrmEasy.Abstractions.Plugins.Enums;
using FakeXrmEasy.Pipeline;
using FakeXrmEasy.Plugins;
using FakeXrmEasy.Plugins.Audit;
using FakeXrmEasy.Plugins.PluginSteps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace D365.Testing
{


    [TestClass]
    public class Dynamics365SalesPluginsUnitTestsPipeline : FakeXrmEasyPipelineTestsBase {

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
        public void TestCreateQuote()
        {
            //Arrange
            var pluginContext = _context.GetDefaultPluginContext();

            pluginContext.MessageName = "Create";

            _context.RegisterPluginStep<Dynamics365.Sales.CPQ.Plugins.GenerateQuote>(new PluginStepDefinition()
            {
                MessageName = Messages.Create.ToString(),
                EntityLogicalName = "quote",
                Stage = ProcessingStepStage.Postoperation,
                Mode = ProcessingStepMode.Synchronous,
                Rank = 1
            });
            _context.RegisterPluginStep<Dynamics365.Sales.QTC.Plugins.CreateQuote>(new PluginStepDefinition()
            {
                MessageName = Messages.Create.ToString(),
                EntityLogicalName = "quote",
                Stage = ProcessingStepStage.Postoperation,
                Mode = ProcessingStepMode.Synchronous,
                Rank = 2
            });

            _service.Execute(new CreateRequest()
            {
                Target = new Entity("quote") { }
            });
            try
            {
                //var result = _context.ExecutePluginWith<Dynamics365.Monitoring.Plugins.ResourceLockPlugin>();
                var pluginStepAudit = _context.GetPluginStepAudit();
                var stepsAudit = pluginStepAudit.CreateQuery().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //Assert.IsTrue(((int)result["rtnInteger"]).Equals(5));
        }


    }
}
