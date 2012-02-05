namespace TestClassesForTests
{
    public class ClassWithByRefArgument
    {
        public ClassWithByRefArgument(out ClassWithOneClassParameter foo)
        {
            foo = null;
        }
    }
}