namespace ConstructorTesterTests.TestClasses
{
    internal class ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly
    {
        public ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly(LonlyAbstractBaseClass foo)
        {
            Guard.AssertNotNull(foo, typeof(LonlyAbstractBaseClass));
        }
    }
}