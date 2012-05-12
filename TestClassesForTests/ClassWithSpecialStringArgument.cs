using System;

namespace TestClassesForTests
{
    public class ClassWithSpecialStringArgument
    {
        public ClassWithSpecialStringArgument(string s)
        {
            if (s != "bar")
                throw new ArgumentException("s has to be bar!");
        }
    }
}