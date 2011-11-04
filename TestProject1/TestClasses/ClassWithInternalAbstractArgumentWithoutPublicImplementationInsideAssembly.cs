namespace ConstructorTesterTests.TestClasses
{
    internal class ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly
    {
        public ClassWithInternalAbstractArgumentWithoutPublicImplementationInsideAssembly(LonelyAbstractBaseClass foo)
        {
            Guard.AssertNotNull(foo, typeof(LonelyAbstractBaseClass));
        }
    }
}