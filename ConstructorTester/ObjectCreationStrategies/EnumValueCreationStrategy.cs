using System;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class EnumValueCreationStrategy : ObjectCreationStrategyBase
    {
        public override bool CanCreate(Type type)
        {
            return type.IsEnum;
        }

        public override object Create(Type type)
        {
            return Enum.GetValues(type).GetValue(0);
        }
    }
}