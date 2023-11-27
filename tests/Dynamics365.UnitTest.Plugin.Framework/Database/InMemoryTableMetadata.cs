using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Database
{
    internal class InMemoryTableMetadata
    {
        protected internal EntityMetadata _entityMetadata;

        protected internal int? _entityTypeCode;
    }
}
