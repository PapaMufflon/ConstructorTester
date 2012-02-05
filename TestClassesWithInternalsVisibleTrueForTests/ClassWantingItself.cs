namespace TestClassesWithInternalsVisibleTrueForTests
{
    internal class ClassWantingItself : AbstractItself
    {
        public ClassWantingItself(AbstractItself publicAbstractBaseClass)
        {

        }
    }

    internal abstract class AbstractItself
    {

    }
}