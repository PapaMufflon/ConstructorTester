using System;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class RegisteredImplementationCreationStrategy : ObjectCreationStrategyBase
    {
        private readonly TestConfig _testConfig;

        public RegisteredImplementationCreationStrategy(TestConfig testConfig)
        {
            _testConfig = testConfig;
        }

        public override bool CanCreate(Type type)
        {
            return _testConfig.HasImplementationFor(type);
        }

        public override object Create(Type type)
        {
            return _testConfig.GetImplementationFor(type);
        }
    }
}