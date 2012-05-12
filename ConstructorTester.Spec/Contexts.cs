using Machine.Fakes;

namespace ConstructorTester.Spec
{
    public class DefaultConfigurationContext
    {
        OnEstablish context = accessor =>
        {
            ArgumentNullTest.Reset();
            ArgumentNullTest.TestInternals = false;
            ArgumentNullTest.TestNullables = false;
        };
    }

    public class TestInternalsContext
    {
        OnEstablish context = accessor =>
        {
            ArgumentNullTest.Reset();
            ArgumentNullTest.TestInternals = true;
            ArgumentNullTest.TestNullables = false;
        };
    }

    public class TestNullablesContext
    {
        OnEstablish context = accessor =>
        {
            ArgumentNullTest.Reset();
            ArgumentNullTest.TestInternals = false;
            ArgumentNullTest.TestNullables = true;
        };
    }
}