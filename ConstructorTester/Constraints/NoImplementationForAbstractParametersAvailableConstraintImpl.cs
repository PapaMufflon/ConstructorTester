using System.Linq;
using System.Reflection;

namespace ConstructorTester.Constraints
{
    internal class NoImplementationForAbstractParametersAvailableConstraintImpl : NoImplementationForParametersAvailableConstraint
    {
        internal override bool CanEvaluateConstructorInfo(ConstructorInfo constructorInfo)
        {
            return constructorInfo.GetParameters().Any(x => x.ParameterType.IsAbstract);
        }
    }
}