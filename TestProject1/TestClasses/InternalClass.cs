namespace ConstructorTesterTests.TestClasses
{
    internal class InternalClass
    {
        public InternalClass(ClassWithoutWrittenConstructor c)
        {
            Guard.AssertNotNull(c, typeof(ClassWithoutWrittenConstructor));
        }
    }
}