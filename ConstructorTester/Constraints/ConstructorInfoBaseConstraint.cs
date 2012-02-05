using System;
using System.Reflection;

namespace ConstructorTester.Constraints
{
    internal abstract class ConstructorInfoBaseConstraint : IConstraint
    {
        public bool CanEvaluate(object @object)
        {
            var constructorInfo = @object as ConstructorInfo;

            return constructorInfo != null && CanEvaluateConstructorInfo(constructorInfo);
        }

        internal virtual bool CanEvaluateConstructorInfo(ConstructorInfo constructorInfo)
        {
            return true;
        }

        public string Evaluate(object @object)
        {
            var result = "";
            var constructorInfo = @object as ConstructorInfo;

            if (constructorInfo == null)
                throw new ArgumentException("Cannot evaluate objects that are not of type ConstructorInfo.");

            result = EvaluateConstructorInfo(constructorInfo);

            return result;
        }

        protected abstract string EvaluateConstructorInfo(ConstructorInfo constructorInfo);
    }
}