using System;

namespace ConstructorTesterTests.TestClasses
{
    public class Guard
    {
        public static void AssertNotNull(object o, Type t)
        {
            if (o == null)
                throw new ArgumentNullException(t.ToString());
        }
    }
}