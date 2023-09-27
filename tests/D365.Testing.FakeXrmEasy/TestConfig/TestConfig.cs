using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeXrmEasy;

namespace D365.Testing.FakeXrmEasy.TestConfig
{
    public class TestConfig
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
