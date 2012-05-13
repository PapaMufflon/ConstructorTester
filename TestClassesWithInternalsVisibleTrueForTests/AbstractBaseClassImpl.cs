namespace TestClassesWithInternalsVisibleTrueForTests
{
    internal class AbstractBaseClassImpl : AbstractBaseClass
    {
        public AbstractBaseClassImpl(ClassWithoutWrittenConstructor c) : base(c) { }
    }
}