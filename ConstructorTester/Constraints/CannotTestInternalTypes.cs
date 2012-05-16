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
        
        public Evaluation Evaluate(object @object)
        {
            var type = @object as Type;

            if (type == null)
                throw new ArgumentException("Cannot evaluate objects that are not Type.");
            
            var result = new Evaluation(type);

            if (!type.IsPublic && !_testConfig.TestInternals)
            {
                result.Failed = true;
                result.Message = "Cannot test non-public classes because TestInternals is set to false.";
            }

            return result;
        }
    }
}