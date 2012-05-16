using System.Linq;
using System.Reflection;

namespace ConstructorTester.Constraints
{
    internal class CannotTestConstructorsWithByRefArguments : ConstructorInfoBaseConstraint
    {
        protected override Evaluation EvaluateConstructorInfo(ConstructorInfo constructorInfo)
        {
            var result = new Evaluation(constructorInfo.DeclaringType);

            if (constructorInfo.GetParameters().Any(x => x.ParameterType.IsByRef))
            {
                result.Failed = true;
                result.Message = "Sorry, ConstructorTester can't test Constructors containing ByRef-arguments.";
            }

            return result;
        }
    }
}