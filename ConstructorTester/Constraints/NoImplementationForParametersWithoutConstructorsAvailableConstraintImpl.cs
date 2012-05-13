using System.Linq;
using System.Reflection;

namespace ConstructorTester.Constraints
{
    internal class NoImplementationForParametersWithoutConstructorsAvailableConstraintImpl : NoImplementationForParametersAvailableConstraint
    {
        internal override bool CanEvaluateConstructorInfo(ConstructorInfo constructorInfo)
        {
            return constructorInfo.GetParameters().Any(x => !x.ParameterType.GetConstructors().Any());
        }
    }
}