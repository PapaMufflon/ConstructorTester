using System;
using System.Linq;
using System.Reflection;
using Rhino.Mocks;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class MockObjectCreationStrategy : ObjectCreationStrategyBase
    {
        private readonly ObjectBuilder _objectBuilder;
        private readonly TestConfig _testConfig;

        public MockObjectCreationStrategy(ObjectBuilder objectBuilder, TestConfig testConfig)
        {
            _objectBuilder = objectBuilder;
            _testConfig = testConfig;
        }

        public override bool CanCreate(Type type)
        {
            return type.IsPublic && IsMockable(type);
        }

        private static bool IsMockable(Type type)
        {
            return (type.IsInterface || type.IsAbstract) &&
                   !type.Attributes.HasFlag(System.Reflection.TypeAttributes.Serializable);
        }

        public override object Create(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (type.IsInterface || constructors.Any(x => x.GetParameters().Length == 0))
            {
                return MockRepository.GenerateStub(type);    
            }
            else
            {
                var constructor = constructors.First(x => x.GetParameters().All(p => _objectBuilder.CanBuildObject(p.ParameterType)));
                var parameters = constructor.GetParameters().Select(x => _objectBuilder.BuildObject(x.ParameterType)).ToArray();
                return MockRepository.GenerateStub(type, parameters);
            }
        }
    }
}