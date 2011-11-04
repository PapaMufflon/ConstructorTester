using System;

namespace ConstructorTesterTests.TestClasses
{
    public class ClassWithDelegate
    {
        public ClassWithDelegate(Func<string, int> foo)
        {
            Guard.AssertNotNull(foo, typeof(Func<string, int>));
        }
    }
}