using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructorTester
{
    internal static class ResultListExtensions
    {
        public static void Evaluate(this List<IResult> self)
        {
            var message = string.Empty;

            var weaknesses = self.OfType<Weakness>().ToList();
            if (weaknesses.Any())
                message = weaknesses.Aggregate("Found possible weaknesses:" + Environment.NewLine, (s, w) => s + string.Format("  In class {0} parameter {1} of constructor {2} was not checked for null." + Environment.NewLine, w.Type, w.ParameterPosition, w.Constructor));

            var evaluations = self.OfType<Evaluation>().ToList();
            if (evaluations.Any())
            {
                var groups = evaluations.GroupBy(e => e.Message);
                
                foreach (var @group in groups)
                {
                    message += @group.First().Message + " Use ArgumentNullTest.Exclude to omit these classes:" + Environment.NewLine;
                    message = @group.Aggregate(message, (current, evaluation) => current + ("  " + evaluation.EvaluatedType + Environment.NewLine));
                }
            }

            var problems = self.OfType<Problem>().ToList();
            if (problems.Any())
                message += problems.Aggregate("ConstructorTester cannot test following classes:" + Environment.NewLine, (s, p) => s + "  " + p.Message + Environment.NewLine);

            if (!string.IsNullOrEmpty(message))
                throw new ArgumentException(message);
        }
    }
}