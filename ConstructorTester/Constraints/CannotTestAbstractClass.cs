using System;

namespace ConstructorTester.Constraints
{
    internal class CannotTestAbstractClass : IConstraint
    {
        public bool CanEvaluate(object @object)
        {
            return @object is Type;
        }

        public Evaluation Evaluate(object @object)
        {
            var type = @object as Type;

            if (type == null)
                throw new ArgumentException("Cannot evaluate objects that are not Type.");
            
            var result = new Evaluation(type);

            if (type.IsAbstract)
            {
                result.Failed = true;
                result.Message = "Sorry, ConstructorTester can't test abstract classes.";
            }

            return result;
        }
    }
}