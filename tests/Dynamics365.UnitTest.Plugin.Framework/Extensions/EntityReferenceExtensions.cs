using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Extensions
{
    public static class EntityReferenceExtensions
    {
        //
        // Parameters:
        //   er:
        public static bool HasKeyAttributes(this EntityReference er)
        {
            if (er == null)
            {
                return false;
            }

            return er.KeyAttributes.Count > 0;
        }
    }
}
