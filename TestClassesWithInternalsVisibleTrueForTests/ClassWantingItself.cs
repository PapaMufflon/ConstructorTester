namespace TestClassesWithInternalsVisibleTrueForTests
{
    internal class ClassWantingItself : AbstractItself
    {
        public ClassWantingItself(AbstractItself publicAbstractBaseClass) { }
    }
}