using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Integrity
{
    public interface IIntegrityOptions
    {
        //
        // Summary:
        //     If true, will validate that when adding/updating an entity reference property
        //     the associated record will exist
        bool ValidateEntityReferences { get; set; }
    }
}
