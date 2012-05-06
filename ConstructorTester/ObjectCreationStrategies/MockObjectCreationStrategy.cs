using System;
using Rhino.Mocks;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class MockObjectCreationStrategy : ObjectCreationStrategyBase
    {
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
            return MockRepository.GenerateStub(type);
        }
    }
}