using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeXrmEasy;
using FakeXrmEasy.Plugins;

namespace D365.Testing.FakeXrmEasy.TestConfig
{
    public class TestConfigBase
    {
        public string GenerateRandomString()
        {
            return "";
        }

        public XrmFakedPluginExecutionContext GeneratePluginContext()
        {
            return new XrmFakedPluginExecutionContext();
        }
    }
}
