﻿using System;
using System.Linq;
using Machine.Fakes;
using Machine.Specifications;
using TestClassesForTests;
using TestClassesWithInternalsVisibleTrueForTests;

namespace ConstructorTester.Spec.SupportedTypes
{
    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_default_ctor_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithDefaultConstructor)));

        protected static Exception _exception;
        Behaves_like<No_Exception> _;
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_one_argument_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithOneClassParameter)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesForTests.ClassWithOneClassParameter parameter 1 of constructor Void .ctor(TestClassesForTests.ClassWithoutWrittenConstructor) was not checked for null." + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_public_class_with_internal_ctor_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<TestInternalsContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithInternalConstructorAndOneClassParameter)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesForTests.ClassWithInternalConstructorAndOneClassParameter parameter 1 of constructor Void .ctor(TestClassesForTests.ClassWithDefaultConstructor) was not checked for null." + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_an_interface_as_argument_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithOneInterfaceParameter)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesForTests.ClassWithOneInterfaceParameter parameter 1 of constructor Void .ctor(TestClassesForTests.IInterface) was not checked for null." + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_a_value_argument_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithOneValueParameter)));

        protected static Exception _exception;
        Behaves_like<No_Exception> _;
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_an_internal_class_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<TestInternalsContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(InternalClass)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesWithInternalsVisibleTrueForTests.InternalClass parameter 1 of constructor Void .ctor(TestClassesWithInternalsVisibleTrueForTests.ClassWithoutWrittenConstructor) was not checked for null." + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_class_based_on_an_abstract_class_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<TestInternalsContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassBasedOnAbstractBaseClass)));

        protected static Exception _exception;
        Behaves_like<Two_failed_assertions> _;

        It should_tell_me_that_the_first_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldContain("In class TestClassesWithInternalsVisibleTrueForTests.ClassBasedOnAbstractBaseClass parameter 1 of constructor Void .ctor(TestClassesWithInternalsVisibleTrueForTests.ClassWithoutWrittenConstructor, TestClassesWithInternalsVisibleTrueForTests.ClassWithoutWrittenConstructor) was not checked for null.");

        It should_tell_me_that_the_second_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldContain("In class TestClassesWithInternalsVisibleTrueForTests.ClassBasedOnAbstractBaseClass parameter 2 of constructor Void .ctor(TestClassesWithInternalsVisibleTrueForTests.ClassWithoutWrittenConstructor, TestClassesWithInternalsVisibleTrueForTests.ClassWithoutWrittenConstructor) was not checked for null.");
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_class_with_a_string_argument_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithOneStringParameter)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesForTests.ClassWithOneStringParameter parameter 1 of constructor Void .ctor(System.String) was not checked for null." + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_class_with_two_ctors_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithTwoConstructors)));

        protected static Exception _exception;
        Behaves_like<Three_failed_assertions> _;

        It should_tell_me_that_the_argument_of_the_first_ctor_was_not_checked_for_null = () =>
            _exception.Message.ShouldContain("In class TestClassesForTests.ClassWithTwoConstructors parameter 1 of constructor Void .ctor(TestClassesForTests.ClassWithoutWrittenConstructor) was not checked for null.");

        It should_tell_me_that_the_first_argument_of_the_second_ctor_was_not_checked_for_null = () =>
            _exception.Message.ShouldContain("In class TestClassesForTests.ClassWithTwoConstructors parameter 1 of constructor Void .ctor(TestClassesForTests.ClassWithoutWrittenConstructor, TestClassesForTests.ClassWithoutWrittenConstructor) was not checked for null.");

        It should_tell_me_that_the_second_argument_of_the_second_ctor_was_not_checked_for_null = () =>
            _exception.Message.ShouldContain("In class TestClassesForTests.ClassWithTwoConstructors parameter 2 of constructor Void .ctor(TestClassesForTests.ClassWithoutWrittenConstructor, TestClassesForTests.ClassWithoutWrittenConstructor) was not checked for null.");
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_an_abstract_argument_without_reachable_implementation_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithNoImplementationForItsAbstractArgument)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesForTests.ClassWithNoImplementationForItsAbstractArgument parameter 1 of constructor Void .ctor(TestClassesForTests.PublicAbsractBaseClassWithoutImplementation) was not checked for null." + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_an_abstract_argument_with_arguments_in_its_constructor_without_reachable_implementation_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithNoImplementationForItsAbstractArgumentWithArgumentsInConstructor)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesForTests.ClassWithNoImplementationForItsAbstractArgumentWithArgumentsInConstructor parameter 1 of constructor Void .ctor(TestClassesForTests.PublicAbstractBaseClassWithoutImplementationButWithConstructorArguments) was not checked for null." + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_an_internal_abstract_argument_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<TestInternalsContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithInternalAbstractArgument)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesWithInternalsVisibleTrueForTests.ClassWithInternalAbstractArgument parameter 1 of constructor Void .ctor(TestClassesWithInternalsVisibleTrueForTests.AbstractBaseClass) was not checked for null." + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_a_nullable_argument_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<TestNullablesContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithNullableArgument)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesForTests.ClassWithNullableArgument parameter 1 of constructor Void .ctor(System.Nullable`1[System.Int32]) was not checked for null." + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_a_nullable_argument_which_cannot_be_converted_from_a_string_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<TestNullablesContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithNullableArgumentInconvertibleFromString)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesForTests.ClassWithNullableArgumentInconvertibleFromString parameter 1 of constructor Void .ctor(System.Nullable`1[System.ComponentModel.ListSortDirection]) was not checked for null." + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_a_generic_list_argument_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithList)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesForTests.ClassWithList parameter 1 of constructor Void .ctor(System.Collections.Generic.List`1[TestClassesForTests.ClassWithOneClassParameter]) was not checked for null." + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_an_enum_argument_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithEnumParameter)));

        protected static Exception _exception;
        Behaves_like<No_Exception> _;
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_a_delegate_argument_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithDelegate)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesForTests.ClassWithDelegate parameter 1 of constructor Void .ctor(System.Func`2[System.String,System.Int32]) was not checked for null." + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_of_an_abstract_class_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<TestInternalsContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(AbstractBaseClass)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_this_is_not_supported_yet = () =>
            _exception.Message.ShouldEqual("Sorry, ConstructorTester can't test abstract classes. Use ArgumentNullTest.Exclude to omit these classes:" + Environment.NewLine + "  TestClassesWithInternalsVisibleTrueForTests.AbstractBaseClass" + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_a_ByRef_argument_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWithByRefArgument)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_this_is_not_supported_yet = () =>
            _exception.Message.ShouldEqual("Sorry, ConstructorTester can't test Constructors containing ByRef-arguments. Use ArgumentNullTest.Exclude to omit these classes:" + Environment.NewLine + "  TestClassesForTests.ClassWithByRefArgument" + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_generic_class_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(Action<>)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_this_is_not_supported_yet = () =>
            _exception.Message.ShouldEqual("Sorry, ConstructorTester can't test generic Constructors. Use ArgumentNullTest.Exclude to omit these classes:" + Environment.NewLine + "  System.Action`1[T]" + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_a_pointer_argument_When_testing_it : WithSubject<object>
    {
        Establish context = () => With<DefaultConfigurationContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(string)));

        protected static Exception _exception;

        It should_tell_me_that_this_is_not_supported_yet = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine +
                                           "  In class System.String parameter 1 of constructor Void .ctor(Char[]) was not checked for null." + Environment.NewLine +
                                           "Sorry, ConstructorTester can't test Constructors containing pointer-arguments. Use ArgumentNullTest.Exclude to omit these classes:" + Environment.NewLine +
                                           "  System.String" + Environment.NewLine +
                                           "  System.String" + Environment.NewLine +
                                           "  System.String" + Environment.NewLine +
                                           "  System.String" + Environment.NewLine +
                                           "  System.String" + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_leading_to_an_endless_loop : WithSubject<object>
    {
        Establish context = () => With<TestInternalsContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(ClassWantingItself)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_no_suitable_implementation_could_be_found = () =>
            _exception.Message.ShouldEqual("ConstructorTester cannot test following classes:" + Environment.NewLine + "  TestClassesWithInternalsVisibleTrueForTests.ClassWantingItself: cannot find an implementation for parameter 1 of constructor Void .ctor(TestClassesWithInternalsVisibleTrueForTests.AbstractItself)" + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_a_serializable_class_argument : WithSubject<object>
    {
        Establish context = () => With<TestInternalsContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(SerializableClassAsConstructorArgument)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesForTests.SerializableClassAsConstructorArgument parameter 1 of constructor Void .ctor(TestClassesForTests.SerializableClass) was not checked for null." + Environment.NewLine);
    }

    [Subject(typeof(ArgumentNullTest))]
    public class Given_a_ctor_with_a_serializable_class_argument_without_deserializer : WithSubject<object>
    {
        Establish context = () => With<TestInternalsContext>();

        Because of = () => _exception = Catch.Exception(() => ArgumentNullTest.Execute(typeof(SerializableClassAsConstructorArgument)));

        protected static Exception _exception;
        Behaves_like<One_failed_assertion> _;

        It should_tell_me_that_the_argument_was_not_checked_for_null = () =>
            _exception.Message.ShouldEqual("Found possible weaknesses:" + Environment.NewLine + "  In class TestClassesForTests.SerializableClassAsConstructorArgument parameter 1 of constructor Void .ctor(TestClassesForTests.SerializableClass) was not checked for null." + Environment.NewLine);
    }
}