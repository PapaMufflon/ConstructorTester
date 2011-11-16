namespace ConstructorTesterTests.TestClasses
{
    public class ClassWithTwoConstructors
    {
        public ClassWithTwoConstructors(ClassWithoutWrittenConstructor c) { }
        public ClassWithTwoConstructors(ClassWithoutWrittenConstructor c, ClassWithoutWrittenConstructor d) { }
    }
}