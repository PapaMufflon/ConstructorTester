using System.Reflection;

namespace ConstructorTester.Constraints
{
    internal class CannotTestGenericConstructors : ConstructorInfoBaseConstraint
    {
        protected override string EvaluateConstructorInfo(ConstructorInfo constructorInfo)
        {
            var result = "";

            if (constructorInfo.ContainsGenericParameters)
                result = "Sorry, ConstructorTester can't test generic Constructors. Use the DoNotTest-method to omit this class.";

            return result;
        }
    }
}