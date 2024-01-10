using FakeItEasy;
using FakeXrmEasy;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Abstractions.Plugins.Enums;
using FakeXrmEasy.Pipeline;
using FakeXrmEasy.Plugins;
using FakeXrmEasy.Plugins.PluginSteps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;
using System.Windows.Controls;

namespace D365.Testing
{
    public class FakeConfigTableRequest : IFakeMessageExecutor
    {
        public bool CanExecute(OrganizationRequest request)
        {
            throw new NotImplementedException();
        }

        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            throw new NotImplementedException();
        }

        public Type GetResponsibleRequestType()
        {
            throw new NotImplementedException();
        }
    }

    [TestClass]
    public class Dynamics365MonitoringPluginsUnitTestsMockAPI : FakeXrmEasyTestsBase {

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
        public void TestMockAPI() {
            //Arrange
            var pluginContext = _context.GetDefaultPluginContext();

            var webservice = A.Fake<D365.SamplePlugin.ExternalWebServicePlugin.IWebService>();
            string response = "this is faked";
            A.CallTo(() => webservice.MakeCall()).Returns(response);

            D365.SamplePlugin.ExternalWebServicePlugin plugin = new SamplePlugin.ExternalWebServicePlugin();
            plugin.myService = webservice;
            
            pluginContext.MessageName = "Update";
            pluginContext.Stage = 40;
            
            string unsecureConfig = "";
            string secureConfig = "";
            try{
                _context.ExecutePluginWith(pluginContext, plugin);
            }
            catch (Exception ex){
                throw ex;
            }
        }
       
    }
}
