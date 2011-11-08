using System.Collections.Generic;
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
            ArgumentNullTest.Execute(typeof(ClassWithInternalConstructorAndOneClassParameter));

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
            ArgumentNullTest.Execute(typeof(InternalClass));

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        [TestMethod]
        public void When_given_a_constructor_calling_an_abstract_base_class_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassBasedOnAbstractBaseClass));

            Assert.AreEqual(_failedAssertions.Count, 2);
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

            Assert.AreEqual(_failedAssertions.Count, 3);
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
            ArgumentNullTest.Execute(typeof(ClassWithInternalAbstractArgument));

            Assert.AreEqual(_failedAssertions.Count, 1);
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
            ArgumentNullTest.Register<LonelyAbstractBaseClass, ImplementationForLonelyAbstractBaseClass>();
            ArgumentNullTest.Execute(typeof(ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly));

            Assert.AreEqual(_failedAssertions.Count, 1);
        }

        private class ImplementationForLonelyAbstractBaseClass : LonelyAbstractBaseClass { }

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
            ArgumentNullTest.Execute(System.Reflection.Assembly.GetExecutingAssembly());

            Assert.AreEqual(_failedAssertions.Count, 16);
        }
    }
}