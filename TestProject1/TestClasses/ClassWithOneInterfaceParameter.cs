namespace ConstructorTesterTests.TestClasses
{
    public class ClassWithOneInterfaceParameter
    {
        public ClassWithOneInterfaceParameter(IInterface foo)
        {
            Guard.AssertNotNull(foo, typeof(IInterface));
        }
    }
}