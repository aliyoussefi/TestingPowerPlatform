using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework.Creation
{
    internal class FakeAndDummyManager
    {
        private static readonly ConcurrentDictionary<Type, Func<ProxyOptions, IFakeOptions>> FakeOptionsFactoryCache = new ConcurrentDictionary<Type, Func<ProxyOptions, IFakeOptions>>();

        private static readonly Action<IFakeOptions> DefaultOptionsBuilder = delegate
        {
        };

        private readonly IFakeObjectCreator fakeCreator;

        private readonly ImplicitOptionsBuilderCatalogue implicitOptionsBuilderCatalogue;

        private readonly IDummyValueResolver dummyValueResolver;

        public FakeAndDummyManager(IDummyValueResolver dummyValueResolver, IFakeObjectCreator fakeCreator, ImplicitOptionsBuilderCatalogue implicitOptionsBuilderCatalogue)
        {
            this.dummyValueResolver = dummyValueResolver;
            this.fakeCreator = fakeCreator;
            this.implicitOptionsBuilderCatalogue = implicitOptionsBuilderCatalogue;
        }

        public object? CreateDummy(Type typeOfDummy, LoopDetectingResolutionContext resolutionContext)
        {
            return dummyValueResolver.TryResolveDummyValue(typeOfDummy, resolutionContext).Result;
        }

        public object CreateFake(Type typeOfFake, LoopDetectingResolutionContext resolutionContext)
        {
            IProxyOptions proxyOptions = BuildProxyOptions(typeOfFake, DefaultOptionsBuilder);
            return fakeCreator.CreateFake(typeOfFake, proxyOptions, dummyValueResolver, resolutionContext).Result;
        }

        public object CreateFake(Type typeOfFake, Action<IFakeOptions> optionsBuilder, LoopDetectingResolutionContext resolutionContext)
        {
            IProxyOptions proxyOptions = BuildProxyOptions(typeOfFake, optionsBuilder);
            return fakeCreator.CreateFake(typeOfFake, proxyOptions, dummyValueResolver, resolutionContext).Result;
        }

        public bool TryCreateDummy(Type typeOfDummy, LoopDetectingResolutionContext resolutionContext, out object? result)
        {
            CreationResult creationResult = dummyValueResolver.TryResolveDummyValue(typeOfDummy, resolutionContext);
            if (creationResult.WasSuccessful)
            {
                result = creationResult.Result;
                return true;
            }

            result = null;
            return false;
        }

        private static IFakeOptions CreateFakeOptions<T>(ProxyOptions proxyOptions) where T : class
        {
            return new FakeOptions<T>(proxyOptions);
        }

        private static Func<ProxyOptions, IFakeOptions> GetFakeOptionsFactory(Type typeOfFake)
        {
            return (Func<ProxyOptions, IFakeOptions>)typeof(FakeAndDummyManager).GetMethod("CreateFakeOptions", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(typeOfFake).CreateDelegate(typeof(Func<ProxyOptions, IFakeOptions>));
        }

        private IProxyOptions BuildProxyOptions(Type typeOfFake, Action<IFakeOptions> optionsBuilder)
        {
            IFakeOptionsBuilder implicitOptionsBuilder = implicitOptionsBuilderCatalogue.GetImplicitOptionsBuilder(typeOfFake);
            if (implicitOptionsBuilder == null && optionsBuilder == DefaultOptionsBuilder)
            {
                return ProxyOptions.Default;
            }

            ProxyOptions proxyOptions = new ProxyOptions();
            IFakeOptions fakeOptions = FakeOptionsFactoryCache.GetOrAdd(typeOfFake, GetFakeOptionsFactory)(proxyOptions);
            if (implicitOptionsBuilder != null)
            {
                try
                {
                    implicitOptionsBuilder.BuildOptions(typeOfFake, fakeOptions);
                }
                catch (Exception innerException)
                {
                    throw new UserCallbackException(ExceptionMessages.UserCallbackThrewAnException($"Fake options builder '{implicitOptionsBuilder.GetType()}'"), innerException);
                }
            }

            optionsBuilder?.Invoke(fakeOptions);
            return proxyOptions;
        }
    }
}
