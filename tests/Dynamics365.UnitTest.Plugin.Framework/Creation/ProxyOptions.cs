using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Creation
{
    internal class ProxyOptions : IProxyOptions
    {
        private class DefaultProxyOptions : IProxyOptions
        {
            public IEnumerable<object?>? ArgumentsForConstructor { get; }

            public ReadOnlyCollection<Type> AdditionalInterfacesToImplement { get; } = new ReadOnlyCollection<Type>(new List<Type>());


            public IEnumerable<Action<object>> ProxyConfigurationActions { get; } = Enumerable.Empty<Action<object>>();


            public IEnumerable<Expression<Func<Attribute>>> Attributes { get; } = Enumerable.Empty<Expression<Func<Attribute>>>();


            public string? Name => null;
        }

        private readonly List<Type> additionalInterfacesToImplement = new List<Type>();

        private readonly List<Action<object>> proxyConfigurationActions = new List<Action<object>>();

        private readonly List<Expression<Func<Attribute>>> attributes = new List<Expression<Func<Attribute>>>();

        public static IProxyOptions Default { get; } = new DefaultProxyOptions();


        public IEnumerable<object?>? ArgumentsForConstructor { get; set; }

        public ReadOnlyCollection<Type> AdditionalInterfacesToImplement => additionalInterfacesToImplement.AsReadOnly();

        public IEnumerable<Action<object>> ProxyConfigurationActions => proxyConfigurationActions;

        public IEnumerable<Expression<Func<Attribute>>> Attributes => attributes;

        public string? Name { get; set; }

        public void AddInterfaceToImplement(Type interfaceType)
        {
            Guard.AgainstNull(interfaceType, "interfaceType");
            if (!interfaceType.GetTypeInfo().IsInterface)
            {
                throw new ArgumentException(ExceptionMessages.NotAnInterface(interfaceType));
            }

            additionalInterfacesToImplement.Add(interfaceType);
        }

        public void AddProxyConfigurationAction(Action<object> action)
        {
            proxyConfigurationActions.Add(action);
        }

        public void AddAttribute(Expression<Func<Attribute>> attribute)
        {
            attributes.Add(attribute);
        }
    }
}
