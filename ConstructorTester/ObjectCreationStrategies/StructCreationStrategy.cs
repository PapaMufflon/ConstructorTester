using System;
using System.Runtime.Serialization;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class StructCreationStrategy : ObjectCreationStrategyBase
    {
        public override bool CanCreate(Type type)
        {
            return type.IsValueType && !type.IsEnum && !type.IsPrimitive;
        }

        public override object Create(Type type)
        {
            return FormatterServices.GetUninitializedObject(type);
        }
    }
}