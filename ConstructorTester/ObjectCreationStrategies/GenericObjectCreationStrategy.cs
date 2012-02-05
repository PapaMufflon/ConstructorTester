using System;
using System.Linq;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class GenericObjectCreationStrategy : IObjectCreationStrategy
    {
        public bool CanCreate(Type type)
        {
            return type.IsGenericTypeDefinition;
        }

        public object Create(Type type)
        {
            return type.MakeGenericType(GetDelegateParameterTypes(type));
        }

        private static Type[] GetDelegateParameterTypes(Type d)
        {
            var invoke = d.GetMethod("Invoke");

            if (invoke == null)
                throw new ApplicationException("Not a delegate.");

            return invoke.GetParameters().Select(x => x.ParameterType).ToArray();
        }
    }
}