using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Rhino.Mocks;

namespace ConstructorTester
{
    public class ArgumentNullTest
    {
        /// <summary>
        /// If set to <c>true</c>, <c>Nullable&lt;T&gt;</c> should also be checked for not null.
        /// </summary>
        public static bool TestNullables { get; set; }

        /// <summary>
        /// If set to <c>true</c>, internal constructors are tested also
        /// (please make sure, ConstructorTester has access to internal classes via the InternalsVisibleTo-attribute).
        /// </summary>
        public static bool TestInternals { get; set; }

        private static Action<string> _failedAssertionAction;
        private static readonly Dictionary<Type, object> _container = new Dictionary<Type, object>();
        private static readonly List<Type> _foundImplementation = new List<Type>();
        private static readonly List<Type> _typesNotToTest = new List<Type>();

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
                TestType(classUnderTest);

            _typesNotToTest.Clear();
        }

        /// <summary>
        /// Tests all constructors of the given <c>Type</c> if all parameters are checked for null.
        /// </summary>
        /// <param name="classUnderTest"><c>Type</c> to check.</param>
        public static void Execute(Type classUnderTest)
        {
            TestType(classUnderTest);

            _typesNotToTest.Clear();
        }

        private static void TestType(Type classUnderTest)
        {
            if (!classUnderTest.IsAbstract &&
                !_typesNotToTest.Contains(classUnderTest) &&
                (TestInternals || classUnderTest.IsPublic))
            {
                var bindingFlags = BindingFlags.Public | BindingFlags.Instance;

                if (TestInternals)
                    bindingFlags = bindingFlags | BindingFlags.NonPublic;

                foreach (var constructor in classUnderTest.GetConstructors(bindingFlags))
                {
                    _foundImplementation.Clear();

                    if (constructor.ContainsGenericParameters)
                    {
                        FailedAssertionAction.Invoke("Sorry, ConstructorTester can't test generic Constructors. Use the DoNotTest-method to omit this class.");
                    }
                    else if (constructor.GetParameters().Any(x => x.ParameterType.IsPointer))
                    {
                        FailedAssertionAction.Invoke("Sorry, ConstructorTester can't test Constructors containing pointer-arguments. Use the DoNotTest-method to omit this class.");
                    }
                    else if (constructor.GetParameters().Any(x => x.ParameterType == typeof(RuntimeArgumentHandle)))
                    {
                        FailedAssertionAction.Invoke("Sorry, ConstructorTester can't test Constructors containing RuntimeArgumentHandle-arguments. Use the DoNotTest-method to omit this class.");
                    }
                    else if (constructor.GetParameters().Any(x => x.ParameterType.IsByRef))
                    {
                        FailedAssertionAction.Invoke("Sorry, ConstructorTester can't test Constructors containing ByRef-arguments. Use the DoNotTest-method to omit this class.");
                    }
                    else
                    {
                        var parameters = GetParameters(constructor.GetParameters());

                        if (parameters != null)
                            TestParameters(constructor, parameters, classUnderTest.ToString());
                        else
                            FailedAssertionAction.Invoke("Could not find all parameters for constructor " + constructor);
                    }
                }
            }
        }

        private static object[] GetParameters(ParameterInfo[] parameterInfo)
        {
            var parameters = new List<object>();

            var stackTrace = new System.Diagnostics.StackTrace();
            if (stackTrace.FrameCount > 50)
            {
                System.Diagnostics.Debugger.Break();
            }

            foreach (var parameterType in parameterInfo.Select(x => x.ParameterType))
            {
                if (_container.ContainsKey(parameterType))
                {
                    parameters.Add(_container[parameterType]);
                }
                else if (parameterType.IsPublic &&
                         (parameterType.IsInterface ||
                         (parameterType.IsAbstract &&
                          parameterType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(x => x.IsPublic).Count() > 0)))
                {
                    parameters.Add(MockRepository.GenerateStub(parameterType));
                }
                else if (parameterType.IsEnum)
                {
                    parameters.Add(Enum.GetValues(parameterType).GetValue(0));
                }
                else if (parameterType.IsValueType) // should be before "no ctors?"
                {
                    parameters.Add(Activator.CreateInstance(parameterType));
                }
                else if (parameterType.IsValueType && !parameterType.IsEnum && !parameterType.IsPrimitive) // is struct?, should be before "no ctors?"
                {
                    parameters.Add(FormatterServices.GetUninitializedObject(parameterType));
                }
                else if ((parameterType.IsAbstract) || (parameterType.GetConstructors().Count() == 0))
                {
                    var implementation = SearchImplementation(parameterType);

                    if (implementation != null)
                        parameters.Add(implementation);
                    else
                        return null;
                }
                else if (Nullable.GetUnderlyingType(parameterType) != null)
                {
                    var nullType = Nullable.GetUnderlyingType(parameterType);
                    parameters.Add(Convert.ChangeType(Activator.CreateInstance(nullType), nullType));
                }
                else if (parameterType == typeof(string))
                {
                    parameters.Add("foo");
                }
                else if (parameterType.IsGenericTypeDefinition)
                {
                    parameters.Add(parameterType.MakeGenericType(GetDelegateParameterTypes(parameterType)));
                }
                else if (parameterType.BaseType == typeof(MulticastDelegate))
                {
                    var returnType = GetDelegateReturnType(parameterType);
                    var methodParameters = parameterType.GetMethod("Invoke").GetParameters().Select(x => x.ParameterType).ToArray();

                    var handler = new DynamicMethod("", returnType, methodParameters, typeof(ArgumentNullTest));
                    var generator = handler.GetILGenerator();

                    generator.Emit(OpCodes.Ldloc, 1);
                    generator.Emit(OpCodes.Ret);

                    parameters.Add(handler.CreateDelegate(parameterType));
                }
                //else if (parameterType.BaseType == typeof(MulticastDelegate))
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
                else
                {
                    try
                    {
                        foreach (var constructor in parameterType.GetConstructors())
                        {
                            if (ProducesDependencyCircle(constructor.GetParameters().Select(x => x.ParameterType)))
                            {
                                //
                            }
                            else if (constructor.ContainsGenericParameters)
                            {
                                //FailedAssertionAction.Invoke("Sorry, ConstructorTester can't test generic Constructors. Use the DoNotTest-method to omit this class.");
                            }
                            else if (constructor.GetParameters().Any(x => x.ParameterType.IsPointer))
                            {
                                //FailedAssertionAction.Invoke("Sorry, ConstructorTester can't test Constructors containing pointer-arguments. Use the DoNotTest-method to omit this class.");
                            }
                            else if (constructor.GetParameters().Any(x => x.ParameterType == typeof(RuntimeArgumentHandle)))
                            {
                                //FailedAssertionAction.Invoke("Sorry, ConstructorTester can't test Constructors containing RuntimeArgumentHandle-arguments. Use the DoNotTest-method to omit this class.");
                            }
                            else if (constructor.GetParameters().Any(x => x.ParameterType.IsByRef))
                            {
                                //FailedAssertionAction.Invoke("Sorry, ConstructorTester can't test Constructors containing ByRef-arguments. Use the DoNotTest-method to omit this class.");
                            }
                            else
                            {
                                _foundImplementation.Add(parameterType);

                                var constructorParameters = GetParameters(constructor.GetParameters());

                                if (constructorParameters != null)
                                {
                                    parameters.Add(constructor.Invoke(constructorParameters));
                                    break;
                                }
                            }
                        }

                        return null;
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
            }

            return parameters.ToArray();
        }

        private static object SearchImplementation(Type internalAbstractType)
        {
            var exceptionsOfFoundImplementations = new Dictionary<ConstructorInfo, Exception>();
            _foundImplementation.Add(internalAbstractType);

            foreach (var type in internalAbstractType.Assembly.GetTypes())
            {
                if (type.BaseType == internalAbstractType && type.GetConstructors().Count() > 0)
                {
                    foreach (var constructor in type.GetConstructors())
                    {
                        if (!ProducesDependencyCircle(constructor.GetParameters().Select(x => x.ParameterType)))
                        {
                            object[] parametersForConstructor = null;

                            try
                            {
                                parametersForConstructor = GetParameters(constructor.GetParameters());
                            }
                            catch (ArgumentException argumentException)
                            {
                                exceptionsOfFoundImplementations.Add(constructor, argumentException);
                                continue;
                            }
                            try
                            {
                                var foundImplementation = constructor.Invoke(parametersForConstructor);
                                return foundImplementation;
                            }
                            catch (Exception e)
                            {
                                exceptionsOfFoundImplementations.Add(constructor, e);
                            }
                        }
                    }
                }
            }

            var errorMessage = string.Format("Cannot find a class implementing {0}.", internalAbstractType);

            if (exceptionsOfFoundImplementations.Count > 0)
                errorMessage =
                    exceptionsOfFoundImplementations.Keys.Aggregate(
                        errorMessage + " But found several possible implementations with these exceptions: \n",
                        (current, exception) =>
                        current + CompileErrorMessage(exceptionsOfFoundImplementations[exception], exception));

            FailedAssertionAction.Invoke(errorMessage);
            return null;
        }

        private static string CompileErrorMessage(Exception exception, ConstructorInfo ctorWhichHasThrownAnException)
        {
            var errorMessage = string.Format("When trying to create a {0}, the exception \"{1}",
                                             ctorWhichHasThrownAnException,
                                             exception.Message);

            var innerException = exception.InnerException;

            while (innerException != null)
            {
                errorMessage += " (With inner exception: " + innerException.Message + ")";

                innerException = innerException.InnerException;
            }

            return errorMessage + "\" was thrown.\n";
        }

        private static bool ProducesDependencyCircle(IEnumerable<Type> parameters)
        {
            return parameters.Any(x => _foundImplementation.Contains(x));
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
            catch (Exception f)
            {
                f = f;
            }

            if (!catched)
                FailedAssertionAction.Invoke(string.Format("Parameter number {0} of constructor {1} of class {2} was not tested for null.", parameterToTest, constructor, classUnderTest));
        }

        /// <summary>
        /// Registers a concrete type to be used for an abstract type or interface.
        /// </summary>
        /// <typeparam name="TBase">The abstract type or interface to be substituted.</typeparam>
        /// <typeparam name="TImplementation">The concrete type which should be used for <c>TBase</c>.</typeparam>
        public static void Register<TBase, TImplementation>()
            where TImplementation : TBase
            where TBase : class
        {
            var constructor = typeof(TImplementation).GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();

            if (constructor != null)
            {
                var parameters = GetParameters(constructor.GetParameters());
                Register<TBase>((TBase)constructor.Invoke(parameters));
            }
        }

        /// <summary>
        /// Registers a concrete object to be used for an abstract type or interface.
        /// </summary>
        /// <typeparam name="T">The abstract type or interface to be substituted.</typeparam>
        /// <param name="implementation">The concrete object which should be used for <c>T</c>.</param>
        public static void Register<T>(T implementation)
        {
            var typeOfT = typeof(T);

            if (_container.ContainsKey(typeOfT))
                _container.Remove(typeOfT);

            _container[typeOfT] = implementation;
        }

        /// <summary>
        /// Empties all registrations made via <c>Register</c>.
        /// </summary>
        public static void DeregisterEverything()
        {
            _container.Clear();
        }

        /// <summary>
        /// Leaves out the given type for the next call to <c>Execute</c>.
        /// </summary>
        /// <param name="type">Type to leave out for testing.</param>
        /// <param name="reason">Give a reason why not to test this type.</param>
        public static void DoNotTest(Type type, string reason)
        {
            if (string.IsNullOrEmpty(reason))
                throw new ArgumentNullException("reason", "Don't be lazy - give me a reason!");

            _typesNotToTest.Add(type);
        }
    }
}