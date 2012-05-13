using System;
using System.Linq;
using System.Reflection.Emit;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class DelegateCreationStrategy : ObjectCreationStrategyBase
    {
        public override bool CanCreate(Type type)
        {
            return type.BaseType == typeof (MulticastDelegate);
        }

        public override object Create(Type type)
        {
            var returnType = GetDelegateReturnType(type);
            var methodParameters = type.GetMethod("Invoke").GetParameters().Select(x => x.ParameterType).ToArray();

            var handler = new DynamicMethod("", returnType, methodParameters, typeof(ArgumentNullTest));
            var generator = handler.GetILGenerator();

            if (returnType != typeof(void))
                generator.Emit(OpCodes.Ldloc, 1);

            generator.Emit(OpCodes.Ret);

            return handler.CreateDelegate(type);
        }

        private static Type GetDelegateReturnType(Type d)
        {
            var invoke = d.GetMethod("Invoke");

            if (invoke == null)
                throw new ApplicationException("Not a delegate.");

            return invoke.ReturnType;
        }
    }
}