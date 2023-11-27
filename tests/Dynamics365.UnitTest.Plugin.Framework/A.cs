using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365.UnitTest.Plugin.Framework
{
    public static class A
    {
        private static FakeAndDummyManager FakeAndDummyManager => ServiceLocator.Resolve<FakeAndDummyManager>();

        private static IFakeConfigurationManager ConfigurationManager => ServiceLocator.Resolve<IFakeConfigurationManager>();

        //
        // Summary:
        //     Creates a fake object of the type T.
        //
        // Type parameters:
        //   T:
        //     The type of fake object to create.
        //
        // Returns:
        //     A fake object.
        public static T Fake<T>() where T : class
        {
            return (T)FakeAndDummyManager.CreateFake(typeof(T), new LoopDetectingResolutionContext());
        }

        //
        // Summary:
        //     Creates a fake object of the type T.
        //
        // Parameters:
        //   optionsBuilder:
        //     A lambda where options for the built fake object can be specified.
        //
        // Type parameters:
        //   T:
        //     The type of fake object to create.
        //
        // Returns:
        //     A fake object.
        public static T Fake<T>(Action<IFakeOptions<T>> optionsBuilder) where T : class
        {
            Action<IFakeOptions<T>> optionsBuilder2 = optionsBuilder;
            Guard.AgainstNull(optionsBuilder2, "optionsBuilder");
            return (T)FakeAndDummyManager.CreateFake(typeof(T), delegate (IFakeOptions options)
            {
                optionsBuilder2((IFakeOptions<T>)options);
            }, new LoopDetectingResolutionContext());
        }

        //
        // Summary:
        //     Creates a collection of fakes of the specified type.
        //
        // Parameters:
        //   numberOfFakes:
        //     The number of fakes in the collection.
        //
        // Type parameters:
        //   T:
        //     The type of fakes to create.
        //
        // Returns:
        //     A collection of fake objects of the specified type.
        public static IList<T> CollectionOfFake<T>(int numberOfFakes) where T : class
        {
            return (from _ in Enumerable.Range(0, numberOfFakes)
                    select Fake<T>()).ToList();
        }

        //
        // Summary:
        //     Creates a collection of fakes of the specified type.
        //
        // Parameters:
        //   numberOfFakes:
        //     The number of fakes in the collection.
        //
        //   optionsBuilder:
        //     A lambda where options for the built fake object can be specified.
        //
        // Type parameters:
        //   T:
        //     The type of fakes to create.
        //
        // Returns:
        //     A collection of fake objects of the specified type.
        public static IList<T> CollectionOfFake<T>(int numberOfFakes, Action<IFakeOptions<T>> optionsBuilder) where T : class
        {
            Action<IFakeOptions<T>> optionsBuilder2 = optionsBuilder;
            return (from _ in Enumerable.Range(0, numberOfFakes)
                    select Fake(optionsBuilder2)).ToList();
        }

        //
        // Summary:
        //     Gets a dummy object of the specified type. The value of a dummy object should
        //     be irrelevant. Dummy objects should not be configured.
        //
        // Type parameters:
        //   T:
        //     The type of dummy to return.
        //
        // Returns:
        //     A dummy object of the specified type. May be null, if a dummy factory is defined
        //     that returns null for dummies of type T.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     Dummies of the specified type can not be created.
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static T Dummy<T>()
        {
            return (T)FakeAndDummyManager.CreateDummy(typeof(T), new LoopDetectingResolutionContext());
        }

        //
        // Summary:
        //     Creates a collection of dummies of the specified type.
        //
        // Parameters:
        //   numberOfDummies:
        //     The number of dummies in the collection.
        //
        // Type parameters:
        //   T:
        //     The type of dummies to create.
        //
        // Returns:
        //     A collection of dummy objects of the specified type. Individual dummies may be
        //     null, if a dummy factory is defined that returns null for dummies of type T.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     Dummies of the specified type can not be created.
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IList<T> CollectionOfDummy<T>(int numberOfDummies)
        {
            return (from _ in Enumerable.Range(0, numberOfDummies)
                    select Dummy<T>()).ToList();
        }

        //
        // Summary:
        //     Configures a call to a faked object.
        //
        // Parameters:
        //   callSpecification:
        //     An expression where the configured member is called.
        //
        // Returns:
        //     A configuration object.
        public static IVoidArgumentValidationConfiguration CallTo(Expression<Action> callSpecification)
        {
            return ConfigurationManager.CallTo(callSpecification);
        }

        //
        // Summary:
        //     Gets a configuration object allowing for further configuration of any call to
        //     the specified faked object.
        //
        // Parameters:
        //   fake:
        //     The fake to configure.
        //
        // Returns:
        //     A configuration object.
        public static IAnyCallConfigurationWithNoReturnTypeSpecified CallTo(object fake)
        {
            return ConfigurationManager.CallTo(fake);
        }

        //
        // Summary:
        //     Configures a call to a faked object.
        //
        // Parameters:
        //   callSpecification:
        //     An expression where the configured member is called.
        //
        // Type parameters:
        //   T:
        //     The type of member on the faked object to configure.
        //
        // Returns:
        //     A configuration object.
        public static IReturnValueArgumentValidationConfiguration<T> CallTo<T>(Expression<Func<T>> callSpecification)
        {
            return ConfigurationManager.CallTo(callSpecification);
        }

        //
        // Summary:
        //     Configures the setting of a property on a faked object.
        //
        // Parameters:
        //   propertySpecification:
        //     An expression that calls the getter of the property to configure.
        //
        // Type parameters:
        //   TValue:
        //     The type of the property value.
        //
        // Returns:
        //     A configuration object.
        public static IPropertySetterAnyValueConfiguration<TValue> CallToSet<TValue>(Expression<Func<TValue>> propertySpecification)
        {
            return ConfigurationManager.CallToSet(propertySpecification);
        }
    }
}
