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


//if (parameterType.BaseType == typeof(MulticastDelegate))
//{
//    var returnType = GetDelegateReturnType(parameterType);
//    var methodParameters = parameterType.GetMethod("Invoke").GetParameters().Select(x => x.ParameterType.ToString()).ToArray();

//    AppDomain domain = AppDomain.CurrentDomain;
//    AssemblyName aname = new AssemblyName("MyEmissions");
//    AssemblyBuilder assemBuilder = domain.DefineDynamicAssembly(aname, AssemblyBuilderAccess.RunAndSave);
//    ModuleBuilder modBuilder = assemBuilder.DefineDynamicModule("MainModule", "MyEmissions.dll");

//    TypeBuilder tb = modBuilder.DefineType("Widget", TypeAttributes.Public);
//    MethodBuilder mb = tb.DefineMethod("Echo", MethodAttributes.Public | MethodAttributes.Static);

//    GenericTypeParameterBuilder[] typeParameters = mb.DefineGenericParameters(methodParameters);
//    //typeParameters[1].SetGenericParameterAttributes(GenericParameterAttributes.ReferenceTypeConstraint);

//    mb.SetReturnType(returnType);
//    mb.SetParameters(typeParameters);

//    ILGenerator gen = mb.GetILGenerator();
//    gen.Emit(OpCodes.Ldnull);
//    gen.Emit(OpCodes.Ret);
//    var dt = tb.CreateType();

//    var mi = dt.GetMethod("Echo");
//    var gm = mi.MakeGenericMethod(new[] { typeof(string), typeof(string) });

//    var parameter = MulticastDelegate.CreateDelegate(typeof(Comparison<string>), gm);
//    try
//    {
//        parameters.Add(parameter);
//    }
//    catch (Exception e)
//    {
//        e = e;
//        throw;
//    }
//}



//private static Type GetDelegateReturnType(Type d)
//{
//    var invoke = d.GetMethod("Invoke");

//    if (invoke == null)
//        throw new ApplicationException("Not a delegate.");

//    return invoke.ReturnType;
//}