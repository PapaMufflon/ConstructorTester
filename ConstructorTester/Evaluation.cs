using System;

namespace ConstructorTester
{
    internal class Evaluation : IResult
    {
        public virtual bool Failed { get; set; }
        public Type EvaluatedType { get; set; }
        public string Message { get; set; }

        public Evaluation(Type evaluatedType)
        {
            EvaluatedType = evaluatedType;
        }
    }
}