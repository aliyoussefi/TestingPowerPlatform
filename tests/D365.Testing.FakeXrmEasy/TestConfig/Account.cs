using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.Testing.FakeXrmEasy.TestConfig
{
    public class Account : Entity
    {
        public string name { get; set; }
        public string branch { get; set; }
        public EntityReference parentaccount { get; set; }
    }
}
