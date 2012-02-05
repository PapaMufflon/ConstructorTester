using System;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class NullableObjectCreationStrategy : ObjectCreationStrategyBase
    {
        public override bool CanCreate(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public override object Create(Type type)
        {
            var nullType = Nullable.GetUnderlyingType(type);
            return Convert.ChangeType(Activator.CreateInstance(nullType), nullType);
        }
    }
}