namespace ConstructorTesterTests.TestClasses
{
    public class ClassWithTwoConstructors
    {
        public ClassWithTwoConstructors(ClassWithoutWrittenConstructor c)
        {
            Guard.AssertNotNull(c, typeof(ClassWithoutWrittenConstructor));
        }

        public ClassWithTwoConstructors(ClassWithoutWrittenConstructor c, ClassWithoutWrittenConstructor d)
        {
            Guard.AssertNotNull(c, typeof(ClassWithoutWrittenConstructor));
            Guard.AssertNotNull(d, typeof(ClassWithoutWrittenConstructor));
        }
    }
}