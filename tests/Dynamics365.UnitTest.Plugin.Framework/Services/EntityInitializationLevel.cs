using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Services
{
    public enum EntityInitializationLevel
    {
        //
        // Summary:
        //     Minimal initialization of common attributes
        Default,
        //
        // Summary:
        //     More detailed initialization of entities, on an entity per entity basis
        PerEntity
    }
}
