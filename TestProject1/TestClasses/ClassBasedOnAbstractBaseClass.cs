namespace ConstructorTesterTests.TestClasses
{
    internal class ClassBasedOnAbstractBaseClass : AbstractBaseClass
    {
        public ClassBasedOnAbstractBaseClass(ClassWithoutWrittenConstructor c, ClassWithoutWrittenConstructor d)
            :base(c)
        {
            Guard.AssertNotNull(d, typeof(ClassWithoutWrittenConstructor));
        }
    }
}