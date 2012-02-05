using System;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class StringCreationStrategy : IObjectCreationStrategy
    {
        public bool CanCreate(Type type)
        {
            return type == typeof (string);
        }

        public object Create(Type type)
        {
            return "foo";
        }
    }
}