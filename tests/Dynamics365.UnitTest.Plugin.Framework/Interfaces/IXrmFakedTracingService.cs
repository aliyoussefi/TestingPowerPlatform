using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Interfaces
{
    public interface IXrmFakedTracingService : ITracingService
    {
        //
        // Summary:
        //     returns all collected trace as a string
        string DumpTrace();
    }
}
