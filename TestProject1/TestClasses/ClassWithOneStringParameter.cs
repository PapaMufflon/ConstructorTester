namespace ConstructorTesterTests.TestClasses
{
    public class ClassWithOneStringParameter
    {
        public ClassWithOneStringParameter(string s)
        {
            Guard.AssertNotNull(s, typeof(string));
        }
    }
}