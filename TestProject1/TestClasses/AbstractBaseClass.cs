namespace ConstructorTesterTests.TestClasses
{
    internal abstract class AbstractBaseClass
    {
        protected AbstractBaseClass(ClassWithoutWrittenConstructor c)
        {
            Guard.AssertNotNull(c, typeof(ClassWithoutWrittenConstructor));
        }
    }
}