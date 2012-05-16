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

        public Evaluation Evaluate(object @object)
        {
            var constructorInfo = @object as ConstructorInfo;

            if (constructorInfo == null)
                throw new ArgumentException("Cannot evaluate objects that are not of type ConstructorInfo.");

            return EvaluateConstructorInfo(constructorInfo);
        }

        protected abstract Evaluation EvaluateConstructorInfo(ConstructorInfo constructorInfo);
    }
}