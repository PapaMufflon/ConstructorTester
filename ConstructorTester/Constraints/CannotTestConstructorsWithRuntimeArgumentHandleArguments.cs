using System;
using System.Linq;
using System.Reflection;

namespace ConstructorTester.Constraints
{
    internal class CannotTestConstructorsWithRuntimeArgumentHandleArguments : ConstructorInfoBaseConstraint
    {
        protected override string EvaluateConstructorInfo(ConstructorInfo constructorInfo)
        {
            var result = "";

            if (constructorInfo.GetParameters().Any(x => x.ParameterType == typeof(RuntimeArgumentHandle)))
                result = "Sorry, ConstructorTester can't test Constructors containing RuntimeArgumentHandle-arguments. Use the DoNotTest-method to omit this class.";

            return result;
        }
    }
}