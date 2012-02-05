using System;
using System.Runtime.Serialization;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class StructCreationStrategy : IObjectCreationStrategy
    {
        public bool CanCreate(Type type)
        {
            return type.IsValueType && !type.IsEnum && !type.IsPrimitive;
        }

        public object Create(Type type)
        {
            return FormatterServices.GetUninitializedObject(type);
        }
    }
}