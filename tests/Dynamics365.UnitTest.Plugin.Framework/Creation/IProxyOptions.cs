using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Creation
{
    internal interface IProxyOptions
    {
        IEnumerable<object?>? ArgumentsForConstructor { get; }

        ReadOnlyCollection<Type> AdditionalInterfacesToImplement { get; }

        IEnumerable<Action<object>> ProxyConfigurationActions { get; }

        IEnumerable<Expression<Func<Attribute>>> Attributes { get; }

        string? Name { get; }
    }
}
