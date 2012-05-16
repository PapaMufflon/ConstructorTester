namespace ConstructorTester.Constraints
{
    internal interface IConstraint
    {
        bool CanEvaluate(object @object);
        Evaluation Evaluate(object @object);
    }
}