namespace TestClassesWithInternalsVisibleTrueForTests
{
    internal class ClassBasedOnAbstractBaseClass : AbstractBaseClass
    {
        public ClassBasedOnAbstractBaseClass(ClassWithoutWrittenConstructor c, ClassWithoutWrittenConstructor d) :base(c) { }
    }
}