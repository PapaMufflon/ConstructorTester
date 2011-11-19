using System;
using System.Linq;
using Machine.Specifications;

namespace ConstructorTester.Spec
{
    [Behaviors]
    public class No_Exception
    {
        protected static Exception _exception;

        It should_not_throw_an_ArgumentException = () => _exception.ShouldBeNull();
    }

    [Behaviors]
    public class One_failed_assertion
    {
        protected static Exception _exception;

        It should_throw_an_ArgumentException = () =>
            _exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Count().ShouldEqual(1);
    }

    [Behaviors]
    public class Two_failed_assertions
    {
        protected static Exception _exception;

        It should_throw_an_ArgumentException = () =>
            _exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Count().ShouldEqual(2);
    }

    [Behaviors]
    public class Three_failed_assertions
    {
        protected static Exception _exception;

        It should_throw_an_ArgumentException = () =>
            _exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Count().ShouldEqual(3);
    }
}