using System;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class NullableObjectCreationStrategy : IObjectCreationStrategy
    {
        public bool CanCreate(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public object Create(Type type)
        {
            var nullType = Nullable.GetUnderlyingType(type);
            return Convert.ChangeType(Activator.CreateInstance(nullType), nullType);
        }
    }
}