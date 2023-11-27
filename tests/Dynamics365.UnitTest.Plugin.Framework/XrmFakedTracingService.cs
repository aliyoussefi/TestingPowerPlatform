using Dynamics365.UnitTest.Plugin.Framework.Interfaces;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework
{
    public class XrmFakedTracingService : IXrmFakedTracingService, ITracingService
    {
        protected StringBuilder _trace { get; set; }

        public XrmFakedTracingService()
        {
            _trace = new StringBuilder();
        }

        //
        // Parameters:
        //   format:
        //
        //   args:
        public void Trace(string format, params object[] args)
        {
            if (args.Length == 0)
            {
                Trace("{0}", format);
            }
            else
            {
                Console.WriteLine(format, args);
                _trace.AppendLine(string.Format(format, args));
            }
        }

        public string DumpTrace()
        {
            return _trace.ToString();
        }
    }
}
