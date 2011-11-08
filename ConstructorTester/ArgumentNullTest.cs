using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Rhino.Mocks;

namespace ConstructorTester
{
    public class ArgumentNullTest
    {
        /// <summary>
        /// If set to <c>true</c>, <c>Nullable&lt;T&gt;</c> should also be checked for not null.
        /// </summary>
        public static bool TestNullables { get; set; }

        private static Action<string> _failedAssertionAction;
        private static readonly Dictionary<Type, object> _container = new Dictionary<Type, object>();

        /// <summary>
        /// If a parameter is found, which is not checked for null, this action is invoked.
        /// You can set your Assertion here, for example:
        /// ArgumentNullTest.FailedAssertionAction = message => Assert.Fail(message);
        /// </summary>
        public static Action<string> FailedAssertionAction
        {
            private get
            {
                if (_failedAssertionAction == null)
                    _failedAssertionAction = Console.WriteLine;

                return _failedAssertionAction;
            }

            set
            {
                _failedAssertionAction = value;
            }
        }

        /// <summary>
        /// Tests all classes in an assembly and all constructors within these classes
        /// if all parameters are checked for null.
        /// </summary>
        /// <param name="assemblyUnderTest"><c>Assembly</c> to check.</param>
        public static void Execute(Assembly assemblyUnderTest)
        {
            foreach (var classUnderTest in assemblyUnderTest.GetTypes())
                Execute(classUnderTest);
        }

        /// <summary>
        /// Tests all constructors of the given <c>Type</c> if all parameters are checked for null.
        /// </summary>
        /// <param name="classUnderTest"><c>Type</c> to check.</param>
        public static void Execute(Type classUnderTest)
        {
            if (!classUnderTest.IsAbstract)
            {
                foreach (var constructor in classUnderTest.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                {
                    var parameters = GetParameters(constructor.GetParameters());
                    TestParameters(constructor, parameters, classUnderTest.ToString());
                }
            }
        }

        private static object[] GetParameters(ParameterInfo[] parameterInfo)
        {
            var parameters = new List<object>();

            foreach (var parameterType in parameterInfo.Select(x => x.ParameterType))
            {
                if (parameterType.IsInterface ||
                    (parameterType.IsAbstract &&
                     parameterType.IsPublic &&
                     parameterType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(x => x.IsPublic).Count() > 0))
                {
                    parameters.Add(MockRepository.GenerateStub(parameterType));
                }
                else if (parameterType.IsAbstract)
                {
                    var implementation = SearchImplementation(parameterType);

                    if (implementation == null && _container.ContainsKey(parameterType))
                        implementation = _container[parameterType];

                    if (implementation != null)
                        parameters.Add(implementation);
                }
                else if (Nullable.GetUnderlyingType(parameterType) != null)
                {
                    var nullType = Nullable.GetUnderlyingType(parameterType);
                    parameters.Add(Convert.ChangeType(Activator.CreateInstance(nullType), nullType));
                }
                else if (parameterType.IsValueType)
                {
                    parameters.Add(Activator.CreateInstance(parameterType));
                }
                else if (parameterType == typeof(string))
                {
                    parameters.Add("");
                }
                else if (parameterType.BaseType == typeof(MulticastDelegate))
                {
                    var returnType = GetDelegateReturnType(parameterType);
                    var handler = new DynamicMethod("", returnType, GetDelegateParameterTypes(parameterType));

                    handler.GetILGenerator().Emit(OpCodes.Ret);
                    parameters.Add(handler.CreateDelegate(parameterType));
                }
                else
                {
                    var constructor = parameterType.GetConstructors()[0];
                    var parametersForConstructor = GetParameters(constructor.GetParameters());
                    parameters.Add(constructor.Invoke(parametersForConstructor));
                }
            }

            return parameters.ToArray();
        }

        private static object SearchImplementation(Type internalAbstractType)
        {
            foreach (var type in internalAbstractType.Assembly.GetTypes())
            {
                if (type.BaseType == internalAbstractType && type.GetConstructors().Count() > 0)
                {
                    var constructor = type.GetConstructors()[0];
                    var parametersForConstructor = GetParameters(constructor.GetParameters());
                    return constructor.Invoke(parametersForConstructor);
                }
            }

            throw new ArgumentException("Cannot find a class implementing " + internalAbstractType);
        }

        private static Type GetDelegateReturnType(Type d)
        {
            var invoke = d.GetMethod("Invoke");
            if (invoke == null)
                throw new ApplicationException("Not a delegate.");

            return invoke.ReturnType;
        }

        private static Type[] GetDelegateParameterTypes(Type d)
        {
            var invoke = d.GetMethod("Invoke");
            if (invoke == null)
                throw new ApplicationException("Not a delegate.");

            return invoke.GetParameters().Select(x => x.ParameterType).ToArray();
        }

        private static void TestParameters(ConstructorInfo constructor, object[] parameters, string classUnderTest)
        {
            for (int parameterCounter = 0; parameterCounter < parameters.Length; parameterCounter++)
            {
                var parameterType = constructor.GetParameters()[parameterCounter].ParameterType;

                if (!parameterType.IsValueType || 
                    (TestNullables && Nullable.GetUnderlyingType(parameterType) != null))
                    TestOneParameterForNull(constructor, parameters, parameterCounter, classUnderTest);
            }
        }

        private static void TestOneParameterForNull(ConstructorInfo constructor, object[] parameters, int parameterToTest, string classUnderTest)
        {
            // copy parameters to not destroy them
            var parametersCopy = (object[])parameters.Clone();
            parametersCopy[parameterToTest] = null;

            var catched = false;

            try
            {
                constructor.Invoke(parametersCopy);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null &&
                    e.InnerException.GetType() == typeof(ArgumentNullException))
                    catched = true;
            }

            if (!catched)
                FailedAssertionAction.Invoke(string.Format("Parameter number {0} of constructor {1} of class {2} was not tested for null.", parameterToTest, constructor, classUnderTest));
        }

        public static void Register<TBase, TImplementation>()
            where TImplementation : TBase
            where TBase : class
        {
            if (_container.ContainsKey(typeof(TBase)))
                _container.Remove(typeof(TBase));

            var constructor = typeof(TImplementation).GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();

            if (constructor != null)
            {
                var parameters = GetParameters(constructor.GetParameters());
                _container[typeof(TBase)] = constructor.Invoke(parameters);
            }
        }
    }
}