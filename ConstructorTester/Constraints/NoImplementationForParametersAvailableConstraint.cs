using System.Reflection;

namespace ConstructorTester.Constraints
{
    internal abstract class NoImplementationForParametersAvailableConstraint : ConstructorInfoBaseConstraint
    {
        protected override string EvaluateConstructorInfo(ConstructorInfo constructorInfo)
        {
            return string.Empty;
        }
    }
}