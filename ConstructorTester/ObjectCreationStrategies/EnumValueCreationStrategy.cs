using System;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class EnumValueCreationStrategy : IObjectCreationStrategy
    {
        public bool CanCreate(Type type)
        {
            return type.IsEnum;
        }

        public object Create(Type type)
        {
            return Enum.GetValues(type).GetValue(0);
        }
    }
}