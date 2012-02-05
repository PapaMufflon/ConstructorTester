using System.Linq;
using System.Reflection;

namespace ConstructorTester.Constraints
{
    internal class CannotTestConstructorsWithByRefArguments : ConstructorInfoBaseConstraint
    {
        protected override string EvaluateConstructorInfo(ConstructorInfo constructorInfo)
        {
            var result = "";

            if (constructorInfo.GetParameters().Any(x => x.ParameterType.IsByRef))
                result = "Sorry, ConstructorTester can't test Constructors containing ByRef-arguments. Use the DoNotTest-method to omit this class.";

            return result;
        }
    }
}