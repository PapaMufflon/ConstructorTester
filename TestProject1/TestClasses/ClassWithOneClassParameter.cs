namespace ConstructorTesterTests.TestClasses
{
    public class ClassWithOneClassParameter
    {
        public ClassWithOneClassParameter(ClassWithoutWrittenConstructor c)
        {
            Guard.AssertNotNull(c, typeof(ClassWithoutWrittenConstructor));
        }
    }
}