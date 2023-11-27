using Microsoft.Xrm.Sdk.PluginTelemetry;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Interfaces.Plugins
{
    public interface IXrmFakedPluginContextProperties
    {
        //
        // Summary:
        //     Organization Service
        IOrganizationService OrganizationService { get; }

        //
        // Summary:
        //     Tracing Service
        IXrmFakedTracingService TracingService { get; }

        //
        // Summary:
        //     Service Endpoint Notification Service
        IServiceEndpointNotificationService ServiceEndpointNotificationService { get; }

        //
        // Summary:
        //     Organization Service Factory
        IOrganizationServiceFactory OrganizationServiceFactory { get; }

        //
        // Summary:
        //     Entity DataSource Retriever Service
        IEntityDataSourceRetrieverService EntityDataSourceRetrieverService { get; }

        //
        // Summary:
        //     Entity DataSource Retriever
        Entity EntityDataSourceRetriever { get; set; }

        //
        // Summary:
        //     Provides a custom implementation for an ILogger interface or returns the current
        //     implementation
        ILogger Logger { get; set; }

        //
        // Summary:
        //     Get the Service Provider for the specified Microsoft.Xrm.Sdk.IPluginExecutionContext
        //
        // Parameters:
        //   plugCtx:
        IServiceProvider GetServiceProvider(IPluginExecutionContext plugCtx);
    }
}
