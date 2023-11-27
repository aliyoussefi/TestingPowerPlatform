using Dynamics365.UnitTest.Plugin.Framework.Integrity;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Middleware
{
    public class MiddlewareBuilder : IMiddlewareBuilder
    {
        private readonly IList<Func<OrganizationRequestDelegate, OrganizationRequestDelegate>> _components = new List<Func<OrganizationRequestDelegate, OrganizationRequestDelegate>>();

        internal IXrmFakedContext _context;

        internal MiddlewareBuilder()
        {
            _context = new XrmFakedContext(this);
        }

        internal MiddlewareBuilder(XrmFakedContext existingContext)
        {
            _context = existingContext;
        }

        //
        // Summary:
        //     New
        public static IMiddlewareBuilder New()
        {
            MiddlewareBuilder middlewareBuilder = new MiddlewareBuilder();
            middlewareBuilder.AddDefaults();
            return middlewareBuilder;
        }

        internal static IMiddlewareBuilder New(XrmFakedContext context)
        {
            MiddlewareBuilder middlewareBuilder = new MiddlewareBuilder(context);
            middlewareBuilder.AddDefaults();
            return middlewareBuilder;
        }

        //
        // Parameters:
        //   addToContextAction:
        public IMiddlewareBuilder Add(Action<IXrmFakedContext> addToContextAction)
        {
            addToContextAction(_context);
            return this;
        }

        private void AddDefaults()
        {
            _context.SetProperty((IIntegrityOptions)new IntegrityOptions
            {
                ValidateEntityReferences = false
            });
        }

        //
        // Parameters:
        //   middleware:
        public IMiddlewareBuilder Use(Func<OrganizationRequestDelegate, OrganizationRequestDelegate> middleware)
        {
            _components.Add(middleware);
            return this;
        }

        //
        // Summary:
        //     Build
        //
        // Exceptions:
        //   T:FakeXrmEasy.Abstractions.Exceptions.LicenseException:
        public IXrmFakedContext Build()
        {
            //if (!_context.LicenseContext.HasValue)
            //{
            //    throw new LicenseException("Please, you need to choose a FakeXrmEasy license. More info at https://dynamicsvalue.github.io/fake-xrm-easy-docs/licensing/licensing-exception/");
            //}

            OrganizationRequestDelegate app = delegate (IXrmFakedContext context, OrganizationRequest request)
            {
                throw UnsupportedExceptionFactory.NotImplementedOrganizationRequest(_context.LicenseContext.Value, request.GetType());
            };
            foreach (Func<OrganizationRequestDelegate, OrganizationRequestDelegate> item in _components.Reverse())
            {
                app = item(app);
            }

            IOrganizationService service = _context.GetOrganizationService();
            A.CallTo(() => service.Execute(A<OrganizationRequest>._)).ReturnsLazily((OrganizationRequest request) => app(_context, request));
            return _context;
        }

        //
        // Parameters:
        //   license:
        public IMiddlewareBuilder SetLicense(FakeXrmEasyLicense license)
        {
            _context.LicenseContext = license;
            return this;
        }
    }
}
