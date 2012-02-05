using System;
using System.Linq;
using Machine.Fakes;
using Machine.Specifications;
using TestClassesForTests;
using TestClassesWithInternalsVisibleTrueForTests;

namespace ConstructorTester.Spec.Features
{
    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_a_nullable_argument_When_testing_it_without_TestNullables_activated : WithSubject<object>
    {
        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithNullableArgument)));

        protected static Exception _exception;
        Behaves_like<No_Exception> _;
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_an_internal_abstract_class_argument_and_an_implementation_for_that_When_testing_it : WithSubject<object>
    {
        Establish context = () =>
        {
            With<TestInternalsContext>();
            ArgumentNullTest.Register<InternalAbstractBaseClassWithoutImplementation, ImplementationForLonelyAbstractBaseClass>();
        };

        private class ImplementationForLonelyAbstractBaseClass : InternalAbstractBaseClassWithoutImplementation { }

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Class TestClassesWithInternalsVisibleTrueForTests.ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly makes trouble: parameter 1 of constructor Void .ctor(TestClassesWithInternalsVisibleTrueForTests.InternalAbstractBaseClassWithoutImplementation) was not tested for null.");
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_an_internal_abstract_class_argument_and_a_concrete_object_for_that_When_testing_it : WithSubject<object>
    {
        Establish context = () =>
        {
            With<TestInternalsContext>();
            ArgumentNullTest.Register<InternalAbstractBaseClassWithoutImplementation>(new ImplementationForLonelyAbstractBaseClass());
        };

        private class ImplementationForLonelyAbstractBaseClass : InternalAbstractBaseClassWithoutImplementation { }

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Class TestClassesWithInternalsVisibleTrueForTests.ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly makes trouble: parameter 1 of constructor Void .ctor(TestClassesWithInternalsVisibleTrueForTests.InternalAbstractBaseClassWithoutImplementation) was not tested for null.");
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_an_internal_abstract_class_argument_and_no_implementation_for_that_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<TestInternalsContext>();

        private class ImplementationForLonelyAbstractBaseClass : InternalAbstractBaseClassWithoutImplementation { }

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_no_suitable_implementation_was_found = () =>
            _exception.Message.ShouldEqual("Class TestClassesWithInternalsVisibleTrueForTests.ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly makes trouble: cannot find an implementation of parameter 1 of constructor Void .ctor(TestClassesWithInternalsVisibleTrueForTests.InternalAbstractBaseClassWithoutImplementation).");
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_an_internal_abstract_class_argument_and_a_deregistered_implementation_for_that_When_testing_it : WithSubject<object>
    {
        Establish context = () =>
        {
            With<TestInternalsContext>();
            ArgumentNullTest.Register<InternalAbstractBaseClassWithoutImplementation>(new ImplementationForLonelyAbstractBaseClass());
            ArgumentNullTest.DeregisterEverything();
        };

        private class ImplementationForLonelyAbstractBaseClass : InternalAbstractBaseClassWithoutImplementation { }

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_no_suitable_implementation_was_found = () =>
            _exception.Message.ShouldEqual("Class TestClassesWithInternalsVisibleTrueForTests.ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly makes trouble: cannot find an implementation of parameter 1 of constructor Void .ctor(TestClassesWithInternalsVisibleTrueForTests.InternalAbstractBaseClassWithoutImplementation).");
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_that_should_not_be_tested_When_testing_it : WithSubject<object>
    {
        Establish context = () =>
        {
            With<DefaultConfigurationContext>();
            ArgumentNullTest.DoNotIncludeInNextTest(typeof(ClassWithOneClassParameter), "I'm testing this feature!");
        };

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithOneClassParameter)));

        protected static Exception _exception;
        Behaves_like<No_Exception> _;
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_that_should_not_be_tested_When_testing_it_twice : WithSubject<object>
    {
        Establish context = () =>
        {
            With<DefaultConfigurationContext>();
            ArgumentNullTest.DoNotIncludeInNextTest(typeof(ClassWithOneClassParameter), "I'm testing this feature!");
        };

        Because of = () =>
        {
            _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithOneClassParameter)));
            _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithOneClassParameter)));
        };

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Class TestClassesForTests.ClassWithOneClassParameter makes trouble: parameter 1 of constructor Void .ctor(TestClassesForTests.ClassWithoutWrittenConstructor) was not tested for null.");
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_an_assembly_with_several_classes_When_testing_it : WithSubject<object>
    {
        private static Exception _exception;

        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithOneClassParameter).Assembly));

        It should_tell_me_all_the_failures = () => _exception.Message.Split(new [] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).Count().ShouldEqual(15);
    }
}