namespace TestClassesForTests
{
    public class ClassWithTwoConstructors
    {
        public ClassWithTwoConstructors(ClassWithoutWrittenConstructor c) { }
        public ClassWithTwoConstructors(ClassWithoutWrittenConstructor c, ClassWithoutWrittenConstructor d) { }
    }
}