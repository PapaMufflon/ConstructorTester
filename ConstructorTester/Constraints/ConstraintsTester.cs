using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructorTester.Constraints
{
    internal class ConstraintsTester
    {
        private readonly IEnumerable<IConstraint> _constraints;
        private readonly TestConfig _testConfig;

        public ConstraintsTester(IEnumerable<IConstraint> constraints, TestConfig testConfig)
        {
            _constraints = constraints;
            _testConfig = testConfig;
        }

        public bool ViolatesConstraints(Type type)
        {
            var violations = EvaluateConstraints(type).Count +
                             type.GetConstructors(_testConfig.GetBindingFlags())
                                 .Aggregate(0, (x, ci) => EvaluateConstraints(ci).Count);

            return violations > 0;
        }

        public ICollection<string> EvaluateConstraints(object @object)
        {
            return _constraints.Where(x => x.CanEvaluate(@object))
                .Select(x => x.Evaluate(@object))
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
        }
    }
}