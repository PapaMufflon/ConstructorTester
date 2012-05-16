using System;
using System.Collections.Generic;
using System.IO;
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
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesWithInternalsVisibleTrueForTests.ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly parameter 1 of constructor Void .ctor(TestClassesWithInternalsVisibleTrueForTests.InternalAbstractBaseClassWithoutImplementation) was not checked for null." + Environment.NewLine);
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
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesWithInternalsVisibleTrueForTests.ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly parameter 1 of constructor Void .ctor(TestClassesWithInternalsVisibleTrueForTests.InternalAbstractBaseClassWithoutImplementation) was not checked for null." + Environment.NewLine);
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
            _exception.Message.ShouldEqual("ConstructorTester cannot test following classes:" + Environment.NewLine + "  TestClassesWithInternalsVisibleTrueForTests.ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly: cannot find an implementation for parameter 1 of constructor Void .ctor(TestClassesWithInternalsVisibleTrueForTests.InternalAbstractBaseClassWithoutImplementation)" + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_an_internal_abstract_class_argument_and_a_deregistered_implementation_for_that_When_testing_it : WithSubject<object>
    {
        Establish context = () =>
        {
            With<TestInternalsContext>();
            ArgumentNullTest.Register<InternalAbstractBaseClassWithoutImplementation>(new ImplementationForLonelyAbstractBaseClass());
            ArgumentNullTest.Reset();
        };

        private class ImplementationForLonelyAbstractBaseClass : InternalAbstractBaseClassWithoutImplementation { }

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_no_suitable_implementation_was_found = () =>
            _exception.Message.ShouldEqual("ConstructorTester cannot test following classes:" + Environment.NewLine + "  TestClassesWithInternalsVisibleTrueForTests.ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly: cannot find an implementation for parameter 1 of constructor Void .ctor(TestClassesWithInternalsVisibleTrueForTests.InternalAbstractBaseClassWithoutImplementation)" + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_that_should_not_be_tested_When_testing_it : WithSubject<object>
    {
        Establish context = () =>
        {
            With<DefaultConfigurationContext>();
            ArgumentNullTest.Exclude(typeof(ClassWithOneClassParameter), "I'm testing this feature!");
        };

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithOneClassParameter)));

        protected static Exception _exception;
        Behaves_like<No_Exception> _;
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_constructor_argument_that_should_not_be_tested_When_testing_the_constructor : WithSubject<object>
    {
        Establish context = () =>
        {
            With<DefaultConfigurationContext>();
            ArgumentNullTest.Exclude(typeof(ClassWithOneClassParameter), "I'm testing this feature!");
        };

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithOneClassParameterWithNonDefaultConstructor)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_no_suitable_implementation_could_be_found = () =>
            _exception.Message.ShouldEqual("ConstructorTester cannot test following classes:" + Environment.NewLine + "  TestClassesForTests.ClassWithOneClassParameterWithNonDefaultConstructor: cannot find an implementation for parameter 1 of constructor Void .ctor(TestClassesForTests.ClassWithOneClassParameter)" + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_constraint_violating_type_in_a_constructor_of_a_constructor_parameter_When_testing_it : WithSubject<object>
    {
        private static Exception _exception;

        Establish context = () => With<TestInternalsContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(System.Threading.Tasks.Task)));

        It should_tell_me_that_no_suitable_implementation_was_found = () =>
            _exception.Message.ShouldContain("7 of constructor Void .ctor(System.Object, System.Object, System.Threading.Tasks.Task, System.Threading.CancellationToken, System.Threading.Tasks.TaskCreationOptions, System.Threading.Tasks.InternalTaskOptions, System.Threading.Tasks.TaskScheduler)");
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_constructor_wanting_a_special_string_as_parameter_When_testing_it : WithSubject<object>
    {
        private static Exception _exception;

        Establish context = () =>
        {
            With<DefaultConfigurationContext>();
            ArgumentNullTest.UseFollowingConstructorParameters<ClassWithSpecialStringArgument>("bar");
        };

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassUsingAClassWithSpecialStringArgument)));

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesForTests.ClassUsingAClassWithSpecialStringArgument parameter 1 of constructor Void .ctor(TestClassesForTests.ClassWithSpecialStringArgument) was not checked for null." + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_two_abstract_constructors_When_testing_it : WithSubject<object>
    {
        private static Exception _exception;

        Establish context = () =>
        {
            With<DefaultConfigurationContext>();
            ArgumentNullTest.UseFollowingConstructorParameters<ClassWithSpecialStringArgument>("bar");
        };

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(new List<Type> { typeof(PublicAbstractClassWithTwoConstructors), typeof(PublicAbstractBaseClass) }));

        It should_tell_me_that_both_constructors_cannot_be_tested = () =>
            _exception.Message.ShouldEqual("Sorry, ConstructorTester can't test abstract classes. Use ArgumentNullTest.Exclude to omit these classes:" + Environment.NewLine + "  TestClassesForTests.PublicAbstractClassWithTwoConstructors" + Environment.NewLine + "  TestClassesForTests.PublicAbstractBaseClass" + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_an_assembly_with_several_classes_When_testing_it : WithSubject<object>
    {
        private static Exception _exception;

        Establish context = () =>
        {
            With<DefaultConfigurationContext>();
            ArgumentNullTest.UseFollowingConstructorParameters<ClassWithSpecialStringArgument>("bar");
        };

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithOneClassParameter).Assembly));

        It should_tell_me_all_the_failures = () => _exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Count().ShouldEqual(27);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_the_System_assembly_When_testing_it : WithSubject<object>
    {
        private static Exception _exception;

        Establish context = () =>
        {
            With<DefaultConfigurationContext>();

            ArgumentNullTest.Register(typeof(string).Assembly);
            ArgumentNullTest.Register(AppDomain.CurrentDomain.ActivationContext);
            ArgumentNullTest.UseFollowingConstructorParameters<System.Globalization.CultureInfo>("en-US");
            ArgumentNullTest.Exclude(typeof(System.IO.FileStream), "Problems with access.");
            ArgumentNullTest.UseFollowingConstructorParameters<System.Security.Principal.WindowsIdentity>(new IntPtr(1));
        };

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(string).Assembly));

        It should_tell_me_all_the_failures = () => _exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Count().ShouldEqual(3086);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_directory_with_several_assemblies_with_several_classes_When_testing_it : WithSubject<object>
    {
        private static Exception _exception;

        Establish context = () =>
        {
            With<DefaultConfigurationContext>();
            ArgumentNullTest.UseFollowingConstructorParameters<ClassWithSpecialStringArgument>("bar");
        };

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(".", "*Test*.dll"));

        It should_tell_me_all_the_failures = () => _exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Count().ShouldEqual(85);
    }
}