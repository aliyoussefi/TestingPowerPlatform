using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Enums
{
    public static class UnsupportedExceptionFactory
    {
        //
        // Summary:
        //     Returns a new general purpose exception based on the current license context
        //
        // Parameters:
        //   currentLicense:
        //
        //   message:
        public static Exception New(string message)
        {
            return new Exception(message);
        }

        //
        // Summary:
        //     Returns a not supported organization request exception
        //
        // Parameters:
        //   currentLicense:
        //
        //   t:
        public static Exception NotImplementedOrganizationRequest(Type t)
        {
            return New($"The organization request type '{t}' is not yet supported... ");
        }

        //
        // Summary:
        //     Returns a partially not supported organization request exception
        //
        // Parameters:
        //   currentLicense:
        //
        //   t:
        //
        //   missingImplementation:
        public static Exception PartiallyNotImplementedOrganizationRequest(Type t, string missingImplementation)
        {
            return New($"The organization request type '{t}' is not yet fully supported... {missingImplementation}...");
        }

        //
        // Summary:
        //     Returns a fetchxml operator not supported exception
        //
        // Parameters:
        //   currentLicense:
        //
        //   op:
        public static Exception FetchXmlOperatorNotImplemented(string op)
        {
            return New("The fetchxml operator '" + op + "' is not yet supported... ");
        }
    }
}
