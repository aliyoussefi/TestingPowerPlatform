using Dynamics365.UnitTest.Plugin.Framework.Interfaces.Plugins;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Interfaces
{
    public interface IXrmBaseContext
    {
        //
        // Summary:
        //     Sets the chosen software license in the FakeXrmEasy framework
        //FakeXrmEasyLicense? LicenseContext { get; set; }

        //
        // Summary:
        //     Returns the caller properties, that is, the default user and business unit used
        //     to impersonate service calls
        ICallerProperties CallerProperties { get; set; }

        //
        // Summary:
        //     PluginContext Properties
        IXrmFakedPluginContextProperties PluginContextProperties { get; set; }

        //
        // Summary:
        //     Returns an instance of an organization service
        IOrganizationService GetOrganizationService();

        //
        // Summary:
        //     Set a value of a specific custom property of a given type
        //
        // Parameters:
        //   property:
        //
        // Type parameters:
        //   T:
        void SetProperty<T>(T property);

        //
        // Summary:
        //     Returns a custom property
        //
        // Returns:
        //     The property requested or exception if property wasn't set before
        T GetProperty<T>();

        //
        // Summary:
        //     True if property was set, false otherwise
        //
        // Type parameters:
        //   T:
        bool HasProperty<T>();

        //
        // Summary:
        //     Returns an instance of a tracing service
        IXrmFakedTracingService GetTracingService();
    }
}
