using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework
{
    public class FakeOrganizationServiceFaultFactory
    {
        //
        // Parameters:
        //   errorCode:
        //
        //   message:
        public static Exception New(ErrorCodes errorCode, string message)
        {
            return new FaultException<OrganizationServiceFault>(new OrganizationServiceFault
            {
                ErrorCode = (int)errorCode,
                Message = message
            }, new FaultReason(message));
        }

        //
        // Parameters:
        //   message:
        public static Exception New(string message)
        {
            return new FaultException<OrganizationServiceFault>(new OrganizationServiceFault
            {
                Message = message
            }, new FaultReason(message));
        }
    }
}
