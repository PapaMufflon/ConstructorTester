using System.Linq;
using System.Reflection;

namespace ConstructorTester.Constraints
{
    internal class CannotTestConstructorsWithPointerArguments : ConstructorInfoBaseConstraint
    {
        protected override string EvaluateConstructorInfo(ConstructorInfo constructorInfo)
        {
            var result = "";

            if (HasPointerArguments(constructorInfo))
                result = "Sorry, ConstructorTester can't test Constructors containing pointer-arguments. Use the DoNotTest-method to omit this class.";

            return result;
        }

        private bool HasPointerArguments(ConstructorInfo constructorInfo)
        {
            return constructorInfo.GetParameters().Any(x => x.ParameterType.IsPointer);
        }
    }
}