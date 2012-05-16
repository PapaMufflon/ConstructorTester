using System.Linq;
using System.Reflection;

namespace ConstructorTester.Constraints
{
    internal class CannotTestConstructorsWithPointerArguments : ConstructorInfoBaseConstraint
    {
        protected override Evaluation EvaluateConstructorInfo(ConstructorInfo constructorInfo)
        {
            var result = new Evaluation(constructorInfo.DeclaringType);

            if (HasPointerArguments(constructorInfo))
            {
                result.Failed = true;
                result.Message = "Sorry, ConstructorTester can't test Constructors containing pointer-arguments.";
            }

            return result;
        }

        private bool HasPointerArguments(ConstructorInfo constructorInfo)
        {
            return constructorInfo.GetParameters().Any(x => x.ParameterType.IsPointer);
        }
    }
}