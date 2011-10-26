namespace ConstructorTesterTests.TestClasses
{
    internal class ClassWithInternalAbstractArgument
    {
        public ClassWithInternalAbstractArgument(AbstractBaseClass foo)
        {
            Guard.AssertNotNull(foo, typeof(AbstractBaseClass));
        }
    }
}