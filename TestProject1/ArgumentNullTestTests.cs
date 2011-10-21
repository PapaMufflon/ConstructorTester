using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConstructorTester;
using ConstructorTesterTests.TestClasses;

namespace ConstructorTesterTests
{
    [TestClass]
    public class ArgumentNullTestTests
    {
        [TestInitialize]
        public void Setup()
        {
            ArgumentNullTest.FailedAssertionAction = s => Assert.Fail(s);
        }

        [TestMethod]
        public void When_given_a_default_constructor_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithDefaultConstructor));
        }

        [TestMethod]
        public void When_given_a_constructor_with_one_class_parameter_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithOneClassParameter));
        }

        [TestMethod]
        public void When_given_an_internal_constructor_with_one_class_parameter_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithInternalConstructorAndOneClassParameter));
        }

        [TestMethod]
        public void When_given_a_constructor_with_one_interface_parameter_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithOneInterfaceParameter));
        }

        [TestMethod]
        public void When_given_a_constructor_with_one_value_parameter_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithOneValueParameter));
        }

        [TestMethod]
        public void When_given_a_constructor_of_an_internal_class_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(InternalClass));
        }

        [TestMethod]
        public void When_given_a_constructor_calling_an_abstract_base_class_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassBasedOnAbstractBaseClass));
        }

        [TestMethod]
        public void When_given_a_constructor_with_a_string_as_parameter_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithOneStringParameter));
        }

        [TestMethod]
        public void When_given_two_constructors_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(typeof(ClassWithTwoConstructors));
        }

        [TestMethod]
        public void When_given_an_abstract_class_Then_it_should_not_throw_an_exception()
        {
            ArgumentNullTest.Execute(typeof(AbstractBaseClass));
        }

        [TestMethod]
        public void When_given_an_assembly_with_several_classes_Then_it_should_handle_this()
        {
            ArgumentNullTest.Execute(System.Reflection.Assembly.GetExecutingAssembly());
        }

    }
}
