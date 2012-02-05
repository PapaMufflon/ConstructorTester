using System;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class ValueTypeCreationStrategy : ObjectCreationStrategyBase
    {
        public override bool CanCreate(Type type)
        {
            return type.IsValueType;
        }

        public override object Create(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}