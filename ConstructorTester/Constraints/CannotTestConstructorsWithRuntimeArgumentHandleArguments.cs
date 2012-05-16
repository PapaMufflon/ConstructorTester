using System;
using System.Linq;
using System.Reflection;

namespace ConstructorTester.Constraints
{
    internal class CannotTestConstructorsWithRuntimeArgumentHandleArguments : ConstructorInfoBaseConstraint
    {
        protected override Evaluation EvaluateConstructorInfo(ConstructorInfo constructorInfo)
        {
            var result = new Evaluation(constructorInfo.DeclaringType);

            if (constructorInfo.GetParameters().Any(x => x.ParameterType == typeof(RuntimeArgumentHandle)))
            {
                result.Failed = true;
                result.Message = "Sorry, ConstructorTester can't test Constructors containing RuntimeArgumentHandle-arguments.";
            }

            return result;
        }
    }
}