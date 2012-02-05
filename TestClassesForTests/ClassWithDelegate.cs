using System;

namespace TestClassesForTests
{
    public class ClassWithDelegate
    {
        public ClassWithDelegate(Func<string, int> foo) { }
    }
}