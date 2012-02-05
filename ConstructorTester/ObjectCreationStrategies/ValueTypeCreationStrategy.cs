using System;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class ValueTypeCreationStrategy : IObjectCreationStrategy
    {
        public bool CanCreate(Type type)
        {
            return type.IsValueType;
        }

        public object Create(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}