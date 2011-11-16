using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConstructorTester;
using ConstructorTesterTests.TestClasses;

namespace ConstructorTesterTests
{
    [TestClass]
    public class ArgumentNullTestTests
    {
        private List<string> _failedAssertions;

        [TestInitialize]
        public void Setup()
        {
            _failedAssertions = new List<string>();
            ArgumentNullTest.FailedAssertionAction = x => _failedAssertions.Add(x);
        }

        [TestMethod]
        public void When_given_a_default_constructor_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithDefaultConstructor));

            Assert.AreEqual(_failedAssertions.Count, 0);
        }

        [TestMethod]
        public void When_given_a_constructor_with_one_class_parameter_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithOneClassParameter));

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        [TestMethod]
        public void When_given_an_internal_constructor_with_one_class_parameter_Then_it_should_handle_this()
        {
            ArgumentNullTest.TestInternals = true;
            ArgumentNullTest.Execute(typeof(ClassWithInternalConstructorAndOneClassParameter));
            ArgumentNullTest.TestInternals = false;

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        [TestMethod]
        public void When_given_a_constructor_with_one_interface_parameter_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithOneInterfaceParameter));

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        [TestMethod]
        public void When_given_a_constructor_with_one_value_parameter_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithOneValueParameter));

            Assert.AreEqual(_failedAssertions.Count, 0);
        }

        [TestMethod]
        public void When_given_a_constructor_of_an_internal_class_Then_it_should_handle_this()
        {
            ArgumentNullTest.TestInternals = true;
            ArgumentNullTest.Execute(typeof(InternalClass));
            ArgumentNullTest.TestInternals = false;

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        [TestMethod]
        public void When_given_a_constructor_calling_an_abstract_base_class_Then_it_should_handle_this()
        {
            ArgumentNullTest.TestInternals = true;
            ArgumentNullTest.Execute(typeof(ClassBasedOnAbstractBaseClass));
            ArgumentNullTest.TestInternals = false;

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        [TestMethod]
        public void When_given_a_constructor_with_a_string_as_parameter_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithOneStringParameter));

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        [TestMethod]
        public void When_given_two_constructors_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithTwoConstructors));

            Assert.AreEqual(_failedAssertions.Count, 2);
        }

        [TestMethod]
        public void When_given_a_constructor_with_an_abstract_class_as_argument_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithAbstractArgument));

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        [TestMethod]
        public void When_given_a_constructor_with_an_internal_abstract_class_as_argument_which_has_an_implementing_public_class_Then_it_should_handle_this()
        {
            ArgumentNullTest.TestInternals = true;
            ArgumentNullTest.Execute(typeof(ClassWithInternalAbstractArgument));
            ArgumentNullTest.TestInternals = false;

            Assert.AreEqual(_failedAssertions.Count, 2);
        }

        [TestMethod]
        public void When_given_an_nullable_argument_Then_it_should_handle_this()
        {
            ArgumentNullTest.TestNullables = true;
            ArgumentNullTest.Execute(typeof(ClassWithNullableArgument));
            ArgumentNullTest.TestNullables = false;

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        [TestMethod]
        public void When_given_an_nullable_argument_and_you_should_not_test_for_nullables_Then_it_should_not_write_an_failed_assertion()
        {
            ArgumentNullTest.Execute(typeof(ClassWithNullableArgument));

            Assert.AreEqual(_failedAssertions.Count, 0);
        }

        [TestMethod]
        public void When_given_an_nullable_argument_which_cannot_be_converted_from_a_string_Then_it_should_handle_this()
        {
            ArgumentNullTest.TestNullables = true;
            ArgumentNullTest.Execute(typeof(ClassWithNullableArgumentInconvertibleFromString));
            ArgumentNullTest.TestNullables = false;

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        [TestMethod]
        public void When_given_a_constructor_with_a_list_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithList));

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        [TestMethod]
        public void When_given_a_constructor_with_an_internal_abstract_class_as_argument_and_an_implementation_for_that_Then_it_should_handle_this()
        {
            ArgumentNullTest.TestInternals = true;
            ArgumentNullTest.Register<LonelyAbstractBaseClass, ImplementationForLonelyAbstractBaseClass>();
            ArgumentNullTest.Execute(typeof(ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly));
            ArgumentNullTest.TestInternals = false;

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        private class ImplementationForLonelyAbstractBaseClass : LonelyAbstractBaseClass { }

        [TestMethod]
        public void When_given_a_concrete_object_for_an_internal_abstract_class_Then_it_should_handle_this()
        {
            ArgumentNullTest.Register<LonelyAbstractBaseClass>(new ImplementationForLonelyAbstractBaseClass());

            ArgumentNullTest.TestInternals = true;
            ArgumentNullTest.Execute(typeof(ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly));
            ArgumentNullTest.TestInternals = false;

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        [TestMethod]
        public void When_given_a_constructor_with_an_internal_abstract_class_as_argument_and_a_deregistered_implementation_for_that_Then_it_tell_that_the_implementation_was_not_found()
        {
            ArgumentNullTest.TestInternals = true;
            ArgumentNullTest.Register<LonelyAbstractBaseClass, ImplementationForLonelyAbstractBaseClass>();
            ArgumentNullTest.DeregisterEverything();

            ArgumentNullTest.Execute(typeof(ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly));
            ArgumentNullTest.TestInternals = false;

            Assert.AreEqual(_failedAssertions.Count, 2);
        }

        [TestMethod]
        public void When_given_a_constructor_with_an_internal_abstract_class_as_argument_and_the_configuration_to_not_test_that_Then_it_should_handle_this()
        {
            ArgumentNullTest.DoNotTest(typeof(ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly), "There is no implementation for this class.");

            ArgumentNullTest.Execute(typeof(ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly));
        }

        [TestMethod]
        public void When_not_testing_a_class_for_the_next_call_but_executing_the_test_twice_Then_it_should_test_the_class_once()
        {
            ArgumentNullTest.DoNotTest(typeof(ClassWithOneClassParameter), "Just testing :)");

            ArgumentNullTest.Execute(typeof(ClassWithOneClassParameter));
            ArgumentNullTest.Execute(typeof(ClassWithOneClassParameter));

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        [TestMethod]
        public void When_given_a_class_with_an_enum_parameter_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithEnumParameter));

            Assert.AreEqual(_failedAssertions.Count, 0);
        }

        [TestMethod]
        public void When_given_a_framework_class_with_a_little_help_Then_it_should_handle_this()
        {
            ArgumentNullTest.Register<Stream, MemoryStream>();
            ArgumentNullTest.Execute(typeof(System.IO.BinaryReader));

            Assert.AreEqual(_failedAssertions.Count, 0);
        }

        [TestMethod]
        public void When_given_a_constructor_with_a_ByRef_argument_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithByRefArgument));

            Assert.AreEqual(_failedAssertions[0], "Sorry, ConstructorTester can't test Constructors containing ByRef-arguments. Use the DoNotTest-method to omit this class.");
        }

        [TestMethod]
        public void When_given_a_framework_class_Then_it_should_say_what_it_needs()
        {
            try
            {
                ArgumentNullTest.DeregisterEverything();
                ArgumentNullTest.Execute(typeof(System.Threading.EventWaitHandle));
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("Cannot find a class implementing System.Boolean&", e.Message);  // ;)
                return;
            }

            Assert.Fail("No ArgumentException was thrown.");
        }

        [TestMethod]
        public void When_given_a_constructor_with_a_delegate_as_argument_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithDelegate));

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        [TestMethod]
        public void When_given_an_abstract_class_Then_it_should_not_throw_an_exception()
        {
            ArgumentNullTest.Execute(typeof(AbstractBaseClass));

            Assert.AreEqual(_failedAssertions.Count, 0);
        }

        [TestMethod]
        public void When_given_an_assembly_with_several_classes_Then_it_should_handle_this()
        {
            ArgumentNullTest.Register<LonelyAbstractBaseClass, ImplementationForLonelyAbstractBaseClass>();

            ArgumentNullTest.Execute(typeof(ClassWithAbstractArgument).Assembly);
            ArgumentNullTest.Execute(typeof(AbstractBaseClass).Assembly);

            Assert.AreEqual(_failedAssertions.Count, 9);
        }

        [TestMethod]
        public void When_given_a_generic_class_Then_it_should_tell_the_user_that_this_is_not_supported_yet()
        {
            ArgumentNullTest.Execute(typeof(Action<>));

            Assert.AreEqual(_failedAssertions[0], "Sorry, ConstructorTester can't test generic Constructors. Use the DoNotTest-method to omit this class.");
        }

        [TestMethod]
        public void When_given_a_pointer_argument_for_a_class_Then_it_should_tell_the_user_that_this_is_not_supported_yet()
        {
            ArgumentNullTest.Execute(typeof(string));

            Assert.AreEqual(_failedAssertions[0], "Sorry, ConstructorTester can't test Constructors containing pointer-arguments. Use the DoNotTest-method to omit this class.");
        }

        [TestMethod]
        public void When_given_a_part_of_the_framework_Then_it_should_handle_this()
        {
            ArgumentNullTest.TestInternals = false;

            ArgumentNullTest.Register(typeof(Type));
            ArgumentNullTest.Register(AppDomain.CurrentDomain);
            ArgumentNullTest.Register(AppDomain.CurrentDomain.ActivationContext);
            ArgumentNullTest.Register(typeof(ArgumentNullTestTests).Assembly);
            ArgumentNullTest.Register(System.Threading.Tasks.TaskScheduler.Current);

            ArgumentNullTest.DoNotTest(typeof(Action<>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Action<,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Action<,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Action<,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Action<,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Action<,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Action<,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Action<,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Action<,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Action<,,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Action<,,,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Action<,,,,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Action<,,,,,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Action<,,,,,,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Action<,,,,,,,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Action<,,,,,,,,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<,,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<,,,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<,,,,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<,,,,,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<,,,,,,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<,,,,,,,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Func<,,,,,,,,,,,,,,,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Comparison<>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Converter<,>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(Predicate<>), "Generics not implemented yet");
            ArgumentNullTest.DoNotTest(typeof(ApplicationIdentity), "Cannot select single ctor to avoid testing yet.");

            ArgumentNullTest.Execute(typeof(string).Assembly);
        }
    }
}