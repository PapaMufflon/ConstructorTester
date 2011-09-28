namespace ConstructorTesterTests.TestClasses
{
    public class ClassWithInternalConstructorAndOneClassParameter
    {
        internal ClassWithInternalConstructorAndOneClassParameter(ClassWithDefaultConstructor c)
        {
            Guard.AssertNotNull(c, typeof(ClassWithDefaultConstructor));
        }
    }
}