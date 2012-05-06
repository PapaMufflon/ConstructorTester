using System;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class StringCreationStrategy : ObjectCreationStrategyBase
    {
        public override bool CanCreate(Type type)
        {
            return type == typeof (string) || type == typeof(object);
        }

        public override object Create(Type type)
        {
            return "foo";
        }
    }
}