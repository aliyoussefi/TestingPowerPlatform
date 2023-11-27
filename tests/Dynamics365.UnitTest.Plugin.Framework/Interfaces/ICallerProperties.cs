using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Interfaces
{
    public interface ICallerProperties
    {
        //
        // Summary:
        //     Default User
        EntityReference CallerId { get; set; }

        //
        // Summary:
        //     Default BusinessUnit
        EntityReference BusinessUnitId { get; set; }
    }
}
