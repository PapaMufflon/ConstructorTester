using System;

namespace ConstructorTester.Constraints
{
    internal class CannotTestAbstractClass : IConstraint
    {
        public bool CanEvaluate(object @object)
        {
            return @object is Type;
        }

        public string Evaluate(object @object)
        {
            var result = "";
            var type = @object as Type;

            if (type == null)
                throw new ArgumentException("Cannot evaluate objects that are not Type.");

            if (type.IsAbstract)
                result = string.Format("Sorry, ConstructorTester can't test abstract classes. Use the DoNotTest-method to omit this class ({0}).", type);

            return result;
        }
    }
}