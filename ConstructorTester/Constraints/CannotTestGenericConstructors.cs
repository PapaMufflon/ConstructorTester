using System.Reflection;

namespace ConstructorTester.Constraints
{
    internal class CannotTestGenericConstructors : ConstructorInfoBaseConstraint
    {
        protected override Evaluation EvaluateConstructorInfo(ConstructorInfo constructorInfo)
        {
            var result = new Evaluation(constructorInfo.DeclaringType);

            if (constructorInfo.ContainsGenericParameters)
            {
                result.Failed = true;
                result.Message = "Sorry, ConstructorTester can't test generic Constructors.";
            }

            return result;
        }
    }
}