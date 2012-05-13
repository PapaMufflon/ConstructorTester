using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rhino.Mocks;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class MockObjectCreationStrategy : ObjectCreationStrategyBase
    {
        private readonly ObjectBuilder _objectBuilder;

        public MockObjectCreationStrategy(ObjectBuilder objectBuilder)
        {
            _objectBuilder = objectBuilder;
        }

        public override bool CanCreate(Type type)
        {
            return type.IsPublic && IsMockable(type);
        }

        private static bool IsMockable(Type type)
        {
            return (type.IsInterface || IsAbstractMockable(type)) &&
                   !type.Attributes.HasFlag(TypeAttributes.Serializable);
        }

        private static bool IsAbstractMockable(Type type)
        {
            return type.IsAbstract && GetSuitableConstructors(type).Any();
        }

        private static IList<ConstructorInfo> GetSuitableConstructors(Type type)
        {
            return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                       .Union(type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.IsFamily))
                       .ToList();
        }

        public override object Create(Type type)
        {
            var constructors = GetSuitableConstructors(type);

            if (CanMockWithoutConstructorArguments(type, constructors))
                return MockRepository.GenerateStub(type);

            var constructor = constructors
                .First(x => x.GetParameters()
                             .All(p => _objectBuilder.CanBuildObject(p.ParameterType)));

            var parameters = constructor
                .GetParameters()
                .Select(x => _objectBuilder.BuildObject(x.ParameterType))
                .ToArray();

            return MockRepository.GenerateStub(type, parameters);
        }

        private static bool CanMockWithoutConstructorArguments(Type type, IEnumerable<ConstructorInfo> constructors)
        {
            return type.IsInterface || constructors.Any(x => x.GetParameters().Length == 0);
        }
    }
}