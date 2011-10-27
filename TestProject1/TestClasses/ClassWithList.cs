using System.Collections.Generic;

namespace ConstructorTesterTests.TestClasses
{
    public class ClassWithList
    {
        public ClassWithList(List<ClassWithOneClassParameter> foo)
        {
            Guard.AssertNotNull(foo, typeof(List<ClassWithOneClassParameter>));
        }
    }
}