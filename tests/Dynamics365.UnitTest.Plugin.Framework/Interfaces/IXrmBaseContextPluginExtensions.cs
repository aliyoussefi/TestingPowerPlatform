using Dynamics365.UnitTest.Plugin.Framework.Interfaces;
using Dynamics365.UnitTest.Plugin.Framework.Interfaces.Plugins;
using Dynamics365.UnitTest.Plugin.Framework.Plugins;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework
{
    public static class IXrmBaseContextPluginExtensions
    {
        //
        // Summary:
        //     Returns a plugin context with default properties one can override
        public static XrmFakedPluginExecutionContext GetDefaultPluginContext(this IXrmBaseContext context)
        {
            Guid guid = context.CallerProperties.CallerId?.Id ?? Guid.NewGuid();
            Guid businessUnitId = context.CallerProperties.BusinessUnitId?.Id ?? Guid.NewGuid();
            return new XrmFakedPluginExecutionContext
            {
                Depth = 1,
                IsExecutingOffline = false,
                MessageName = "Create",
                UserId = guid,
                BusinessUnitId = businessUnitId,
                InitiatingUserId = guid,
                InputParameters = new ParameterCollection(),
                OutputParameters = new ParameterCollection(),
                SharedVariables = new ParameterCollection(),
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection(),
                IsolationMode = 1
            };
        }

        //
        // Summary:
        //     Returns the default plugin context properties of this IXrmBaseContext
        //
        // Parameters:
        //   context:
        public static IXrmFakedPluginContextProperties GetPluginContextProperties(this IXrmBaseContext context)
        {
            if (context.PluginContextProperties == null)
            {
                context.PluginContextProperties = new XrmFakedPluginContextProperties(context, context.GetOrganizationService(), context.GetTracingService());
            }

            return context.PluginContextProperties;
        }

        //
        // Summary:
        //     Executes a plugin passing a custom context. This is useful whenever we need to
        //     mock more complex plugin contexts (ex: passing MessageName, plugin Depth, InitiatingUserId
        //     etc...)
        //
        // Parameters:
        //   context:
        //     The IXrmBaseContext instance where this plugin will be executed
        //
        //   plugCtx:
        //     A plugin context with the minimum set of properties needed for the plugin execution
        //
        //   instance:
        //     A specific plugin instance, where you might have already setup / injected other
        //     dependencies
        public static IPlugin ExecutePluginWith(this IXrmBaseContext context, XrmFakedPluginExecutionContext plugCtx, IPlugin instance)
        {
            IServiceProvider fakedServiceProvider = context.GetPluginContextProperties().GetServiceProvider(plugCtx);
            IPlugin fakedPlugin = A.Fake<IPlugin>();
            A.CallTo(() => fakedPlugin.Execute(A<IServiceProvider>._)).Invokes<IVoidConfiguration, IServiceProvider>(delegate
            {
                IPlugin plugin = instance;
                plugin.Execute(fakedServiceProvider);
            });
            fakedPlugin.Execute(fakedServiceProvider);
            return fakedPlugin;
        }

        //
        // Summary:
        //     Executes a plugin passing a custom context. This is useful whenever we need to
        //     mock more complex plugin contexts (ex: passing MessageName, plugin Depth, InitiatingUserId
        //     etc...)
        //
        // Parameters:
        //   context:
        //     The IXrmBaseContext instance where this plugin will be executed
        //
        //   ctx:
        //     A plugin context with the minimum set of properties needed for the plugin execution
        //
        // Type parameters:
        //   T:
        //     Must be a plugin
        public static IPlugin ExecutePluginWith<T>(this IXrmBaseContext context, XrmFakedPluginExecutionContext ctx = null) where T : IPlugin, new()
        {
            if (ctx == null)
            {
                ctx = context.GetDefaultPluginContext();
            }

            return context.ExecutePluginWith(ctx, new T());
        }

        //
        // Summary:
        //     Method to execute a plugin that takes several plugin execution properties as
        //     parameters. Soon to be deprecated, use the ExecutePluginWith<T> that takes a
        //     plugin context instance instead
        //
        // Parameters:
        //   context:
        //
        //   inputParameters:
        //
        //   outputParameters:
        //
        //   preEntityImages:
        //
        //   postEntityImages:
        //
        // Type parameters:
        //   T:
        public static IPlugin ExecutePluginWith<T>(this IXrmBaseContext context, ParameterCollection inputParameters, ParameterCollection outputParameters, EntityImageCollection preEntityImages, EntityImageCollection postEntityImages) where T : IPlugin, new()
        {
            XrmFakedPluginExecutionContext defaultPluginContext = context.GetDefaultPluginContext();
            defaultPluginContext.InputParameters.AddRange(inputParameters);
            defaultPluginContext.OutputParameters.AddRange(outputParameters);
            defaultPluginContext.PreEntityImages.AddRange(preEntityImages);
            defaultPluginContext.PostEntityImages.AddRange(postEntityImages);
            IServiceProvider fakedServiceProvider = context.GetPluginContextProperties().GetServiceProvider(defaultPluginContext);
            IPlugin fakedPlugin = A.Fake<IPlugin>();
            A.CallTo(() => fakedPlugin.Execute(A<IServiceProvider>._)).Invokes<IVoidConfiguration, IServiceProvider>(delegate
            {
                new T().Execute(fakedServiceProvider);
            });
            fakedPlugin.Execute(fakedServiceProvider);
            return fakedPlugin;
        }

        //
        // Summary:
        //     Executes a plugin with the unsecure and secure configurations specified
        //
        // Parameters:
        //   context:
        //
        //   plugCtx:
        //
        //   unsecureConfiguration:
        //
        //   secureConfiguration:
        //
        // Type parameters:
        //   T:
        public static IPlugin ExecutePluginWithConfigurations<T>(this IXrmBaseContext context, XrmFakedPluginExecutionContext plugCtx, string unsecureConfiguration, string secureConfiguration) where T : class, IPlugin
        {
            Type typeFromHandle = typeof(T);
            List<ConstructorInfo> source = typeFromHandle.GetConstructors().ToList();
            if (!source.Any((ConstructorInfo c) => c.GetParameters().Length == 2 && c.GetParameters().All((ParameterInfo param) => param.ParameterType == typeof(string))))
            {
                throw new ArgumentException("The plugin you are trying to execute does not specify a constructor for passing in two configuration strings.");
            }

            T instance = (T)Activator.CreateInstance(typeof(T), unsecureConfiguration, secureConfiguration);
            return context.ExecutePluginWith(plugCtx, instance);
        }

        //
        // Summary:
        //     Method to execute a plugin with configurations that also takes a specific plugin
        //     instance where you might already injected other external dependencies
        //
        // Parameters:
        //   context:
        //
        //   plugCtx:
        //
        //   instance:
        //
        //   unsecureConfiguration:
        //
        //   secureConfiguration:
        //
        // Type parameters:
        //   T:
        [Obsolete("Use ExecutePluginWith(XrmFakedPluginExecutionContext ctx, IPlugin instance).")]
        public static IPlugin ExecutePluginWithConfigurations<T>(this IXrmBaseContext context, XrmFakedPluginExecutionContext plugCtx, T instance, string unsecureConfiguration = "", string secureConfiguration = "") where T : class, IPlugin
        {
            IServiceProvider fakedServiceProvider = context.PluginContextProperties.GetServiceProvider(plugCtx);
            IPlugin fakedPlugin = A.Fake<IPlugin>();
            A.CallTo(() => fakedPlugin.Execute(A<IServiceProvider>._)).Invokes<IVoidConfiguration, IServiceProvider>(delegate
            {
                Type typeFromHandle = typeof(T);
                ConstructorInfo[] constructors = typeFromHandle.GetConstructors();
                if (!constructors.Any((ConstructorInfo c) => c.GetParameters().Length == 2 && c.GetParameters().All((ParameterInfo param) => param.ParameterType == typeof(string))))
                {
                    throw new ArgumentException("The plugin you are trying to execute does not specify a constructor for passing in two configuration strings.");
                }

                T val = instance;
                val.Execute(fakedServiceProvider);
            });
            fakedPlugin.Execute(fakedServiceProvider);
            return fakedPlugin;
        }

        //
        // Summary:
        //     Executes a plugin with a given target
        //
        // Parameters:
        //   context:
        //
        //   ctx:
        //
        //   target:
        //
        //   messageName:
        //
        //   stage:
        //
        // Type parameters:
        //   T:
        public static IPlugin ExecutePluginWithTarget<T>(this IXrmBaseContext context, XrmFakedPluginExecutionContext ctx, Entity target, string messageName = "Create", int stage = 40) where T : IPlugin, new()
        {
            ctx.InputParameters.Add("Target", target);
            ctx.MessageName = messageName;
            ctx.Stage = stage;
            return context.ExecutePluginWith<T>(ctx);
        }

        //
        // Summary:
        //     Executes the plugin of type T against the faked context for an entity target
        //     and returns the faked plugin
        //
        // Parameters:
        //   context:
        //
        //   target:
        //     The entity to execute the plug-in for.
        //
        //   messageName:
        //     Sets the message name.
        //
        //   stage:
        //     Sets the stage.
        //
        // Type parameters:
        //   T:
        public static IPlugin ExecutePluginWithTarget<T>(this IXrmBaseContext context, Entity target, string messageName = "Create", int stage = 40) where T : IPlugin, new()
        {
            return context.ExecutePluginWithTarget(new T(), target, messageName, stage);
        }

        //
        // Summary:
        //     Executes the plugin of type T against the faked context for an entity target
        //     and returns the faked plugin
        //
        // Parameters:
        //   context:
        //
        //   instance:
        //
        //   target:
        //     The entity to execute the plug-in for.
        //
        //   messageName:
        //     Sets the message name.
        //
        //   stage:
        //     Sets the stage.
        public static IPlugin ExecutePluginWithTarget(this IXrmBaseContext context, IPlugin instance, Entity target, string messageName = "Create", int stage = 40)
        {
            XrmFakedPluginExecutionContext defaultPluginContext = context.GetDefaultPluginContext();
            defaultPluginContext.InputParameters.Add("Target", target);
            defaultPluginContext.MessageName = messageName;
            defaultPluginContext.Stage = stage;
            return context.ExecutePluginWith(defaultPluginContext, instance);
        }

        //
        // Summary:
        //     Executes the plugin of type T against the faked context for an entity reference
        //     target and returns the faked plugin
        //
        // Parameters:
        //   context:
        //
        //   target:
        //     The entity reference to execute the plug-in for.
        //
        //   messageName:
        //     Sets the message name.
        //
        //   stage:
        //     Sets the stage.
        //
        // Type parameters:
        //   T:
        public static IPlugin ExecutePluginWithTargetReference<T>(this IXrmBaseContext context, EntityReference target, string messageName = "Delete", int stage = 40) where T : IPlugin, new()
        {
            return context.ExecutePluginWithTargetReference(new T(), target, messageName, stage);
        }

        //
        // Summary:
        //     Executes the plugin of type T against the faked context for an entity reference
        //     target and returns the faked plugin
        //
        // Parameters:
        //   context:
        //     The IXrmBaseContext used to execute the plugin
        //
        //   instance:
        //     A specific plugin instance
        //
        //   target:
        //     The entity reference to execute the plug-in for.
        //
        //   messageName:
        //     Sets the message name.
        //
        //   stage:
        //     Sets the stage.
        public static IPlugin ExecutePluginWithTargetReference(this IXrmBaseContext context, IPlugin instance, EntityReference target, string messageName = "Delete", int stage = 40)
        {
            XrmFakedPluginExecutionContext defaultPluginContext = context.GetDefaultPluginContext();
            defaultPluginContext.InputParameters.Add("Target", target);
            defaultPluginContext.MessageName = messageName;
            defaultPluginContext.Stage = stage;
            return context.ExecutePluginWith(defaultPluginContext, instance);
        }

        //
        // Summary:
        //     Returns a faked plugin with a target and the specified pre entity images
        //
        // Type parameters:
        //   T:
        [Obsolete("Use ExecutePluginWith<T> instead")]
        public static IPlugin ExecutePluginWithTargetAndPreEntityImages<T>(this IXrmBaseContext context, object target, EntityImageCollection preEntityImages, string messageName = "Create", int stage = 40) where T : IPlugin, new()
        {
            XrmFakedPluginExecutionContext defaultPluginContext = context.GetDefaultPluginContext();
            defaultPluginContext.InputParameters.Add("Target", target);
            defaultPluginContext.PreEntityImages.AddRange(preEntityImages);
            defaultPluginContext.MessageName = messageName;
            defaultPluginContext.Stage = stage;
            return context.ExecutePluginWith<T>(defaultPluginContext);
        }

        //
        // Summary:
        //     Returns a faked plugin with a target and the specified post entity images
        //
        // Parameters:
        //   context:
        //
        //   target:
        //
        //   postEntityImages:
        //
        //   messageName:
        //
        //   stage:
        //
        // Type parameters:
        //   T:
        [Obsolete("Use ExecutePluginWith<T> instead")]
        public static IPlugin ExecutePluginWithTargetAndPostEntityImages<T>(this IXrmBaseContext context, object target, EntityImageCollection postEntityImages, string messageName = "Create", int stage = 40) where T : IPlugin, new()
        {
            XrmFakedPluginExecutionContext defaultPluginContext = context.GetDefaultPluginContext();
            defaultPluginContext.InputParameters.Add("Target", target);
            defaultPluginContext.PostEntityImages.AddRange(postEntityImages);
            defaultPluginContext.MessageName = messageName;
            defaultPluginContext.Stage = stage;
            return context.ExecutePluginWith<T>(defaultPluginContext);
        }

        //
        // Parameters:
        //   context:
        //
        //   target:
        //
        //   inputParameters:
        //
        //   messageName:
        //
        //   stage:
        //
        // Type parameters:
        //   T:
        [Obsolete("Use ExecutePluginWith<T> instead")]
        public static IPlugin ExecutePluginWithTargetAndInputParameters<T>(this IXrmBaseContext context, Entity target, ParameterCollection inputParameters, string messageName = "Create", int stage = 40) where T : IPlugin, new()
        {
            XrmFakedPluginExecutionContext defaultPluginContext = context.GetDefaultPluginContext();
            defaultPluginContext.InputParameters.AddRange(inputParameters);
            return context.ExecutePluginWithTarget<T>(defaultPluginContext, target, messageName, stage);
        }
    }
}
