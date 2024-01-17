using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Contexts;
using System.Text;
using D365.Testing.FakeXrmEasy.TestConfig;
using FakeXrmEasy;
using FakeXrmEasy.Plugins;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace D365.Testing{
    [TestClass]
    public class D365SamplePluginUnitTestsRetrieveMultiple : FakeXrmEasyTestsBase
    {

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
        public void TestFakedRetrieveMultiple()
        {

            //Arrange
            var pluginContext = _context.GetDefaultPluginContext();

            //XrmFakedContext context = new XrmFakedContext();
            //IOrganizationService service = context.GetOrganizationService();

            D365.SamplePlugin.RetrieveMultiplePlugin plugin = new SamplePlugin.RetrieveMultiplePlugin();

            pluginContext.MessageName = "RetrieveMultiple";
            pluginContext.Stage = 40;

            string unsecureConfig = "";
            string secureConfig = "";

            List<Entity> initialEntities = new List<Entity>();
            int excessNumberOfRecords = 50;
            //_context.MaxRetrieveCount = 1000;
            //_context.
            for (int i = 0; i < 1000 + excessNumberOfRecords; i++)
            {
                Entity e = new Entity("account");
                e.Id = Guid.NewGuid();
                initialEntities.Add(e);
            }
            _context.Initialize(initialEntities);

            try
            {
                _context.ExecutePluginWith(pluginContext, plugin);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
