using Machine.Fakes;

namespace ConstructorTester.Spec
{
    public class DefaultConfigurationContext
    {
        OnEstablish context = accessor =>
        {
            ArgumentNullTest.DeregisterEverything();
            ArgumentNullTest.TestInternals = false;
            ArgumentNullTest.TestNullables = false;
        };
    }

    public class TestInternalsContext
    {
        OnEstablish context = accessor =>
        {
            ArgumentNullTest.DeregisterEverything();
            ArgumentNullTest.TestInternals = true;
            ArgumentNullTest.TestNullables = false;
        };
    }

    public class TestNullablesContext
    {
        OnEstablish context = accessor =>
        {
            ArgumentNullTest.DeregisterEverything();
            ArgumentNullTest.TestInternals = false;
            ArgumentNullTest.TestNullables = true;
        };
    }
}