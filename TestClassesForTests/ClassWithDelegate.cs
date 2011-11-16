using System;

namespace ConstructorTesterTests.TestClasses
{
    public class ClassWithDelegate
    {
        public ClassWithDelegate(Func<string, int> foo) { }
    }
}