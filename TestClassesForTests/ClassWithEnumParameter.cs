namespace ConstructorTesterTests.TestClasses
{
    public class ClassWithEnumParameter
    {
        public ClassWithEnumParameter(FooEnum foo) { }
    }

    public enum FooEnum
    {
        Foo = 37,
        Bar = 13
    }
}