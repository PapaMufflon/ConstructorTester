using System;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class RegisteredImplementationCreationStrategy : IObjectCreationStrategy
    {
        private readonly TestConfig _testConfig;

        public RegisteredImplementationCreationStrategy(TestConfig testConfig)
        {
            _testConfig = testConfig;
        }

        public bool CanCreate(Type type)
        {
            return _testConfig.HasImplementationFor(type);
        }

        public object Create(Type type)
        {
            return _testConfig.GetImplementationFor(type);
        }
    }
}