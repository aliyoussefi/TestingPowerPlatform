using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Middleware
{
    public delegate OrganizationResponse OrganizationRequestDelegate(IXrmFakedContext context, OrganizationRequest request);
    public interface IMiddlewareBuilder
    {
        //
        // Summary:
        //     Adds a custom setup action to the context. All the Add methods should be called
        //     before any Use method.
        //
        // Parameters:
        //   context:
        IMiddlewareBuilder Add(Action<IXrmFakedContext> context);

        //
        // Summary:
        //     Use this method to choose between the available FakeXrmEasy licenses
        //
        // Parameters:
        //   license:
        //IMiddlewareBuilder SetLicense(FakeXrmEasyLicense license);

        //
        // Summary:
        //     Adds a new delegate to the middleware. The order in which the delegates are "used"
        //     is important. They'll define the pipeline sequence basically.
        //
        // Parameters:
        //   middleware:
        IMiddlewareBuilder Use(Func<OrganizationRequestDelegate, OrganizationRequestDelegate> middleware);

        //
        // Summary:
        //     Builds a new context with all the setup actions (Add) and Use methods defined.
        IXrmFakedContext Build();
    }
}
