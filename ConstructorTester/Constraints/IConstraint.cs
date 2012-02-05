namespace ConstructorTester.Constraints
{
    internal interface IConstraint
    {
        bool CanEvaluate(object @object);
        string Evaluate(object @object);
    }
}