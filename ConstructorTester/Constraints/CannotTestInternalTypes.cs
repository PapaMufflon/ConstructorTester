using System;

namespace ConstructorTester.Constraints
{
    internal class CannotTestNonPublicTypesWhenTestInternalsIsFalse : IConstraint
    {
        private readonly TestConfig _testConfig;

        public CannotTestNonPublicTypesWhenTestInternalsIsFalse(TestConfig testConfig)
        {
            _testConfig = testConfig;
        }

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

            if (!type.IsPublic && !_testConfig.TestInternals)
                result = string.Format("Cannot test non-public class {0} because TestInternals is set to false.", type);

            return result;
        }
    }
}