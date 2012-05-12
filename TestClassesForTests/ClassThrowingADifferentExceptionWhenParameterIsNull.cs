using System;

namespace TestClassesForTests
{
    public class ClassThrowingADifferentExceptionWhenParameterIsNull
    {
        public ClassThrowingADifferentExceptionWhenParameterIsNull(object o)
        {
            if (o == null)
                throw new ArgumentException();
        }
    }
}