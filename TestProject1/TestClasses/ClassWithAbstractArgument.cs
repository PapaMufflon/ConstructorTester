namespace ConstructorTesterTests.TestClasses
{
    public class ClassWithAbstractArgument
    {
        public ClassWithAbstractArgument(PublicAbstractBaseClass foo)
        {
            Guard.AssertNotNull(foo, typeof(PublicAbstractBaseClass));
        }
    }
}