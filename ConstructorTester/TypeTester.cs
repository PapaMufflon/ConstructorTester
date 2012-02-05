using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using ConstructorTester.Constraints;

namespace ConstructorTester
{
    internal class TypeTester
    {
        private readonly TestConfig _testConfig;
        private readonly List<IConstraint> _constraints;
        private readonly ObjectBuilder _objectBuilder;
        private readonly List<string> _failedAssertions = new List<string>();
        private readonly List<Type> _foundImplementation = new List<Type>();

        public TypeTester(TestConfig testConfig, List<IConstraint> constraints, ObjectBuilder objectBuilder)
        {
            _testConfig = testConfig;
            _constraints = constraints;
            _objectBuilder = objectBuilder;
        }

        public IEnumerable<string> TestForNullArgumentExceptionsInConstructor(Type type)
        {
            _failedAssertions.Clear();

            _failedAssertions.AddRange(EvaluateConstraints(type));

            if (_failedAssertions.Count == 0)
                TestType(type);

            return _failedAssertions;
        }

        private ICollection<string> EvaluateConstraints(object @object)
        {
            return _constraints.Where(x => x.CanEvaluate(@object))
                .Select(x => x.Evaluate(@object))
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
        }

        private void TestType(Type type)
        {
            var bindingFlags = GetBindingFlags();

            foreach (var constructor in type.GetConstructors(bindingFlags))
            {
                _foundImplementation.Clear();

                var failedAssertionsForCurrentConstructor = EvaluateConstraints(constructor);

                if (failedAssertionsForCurrentConstructor.Count() == 0)
                {
                    var parameters = GetParameters(constructor);

                    if (parameters != null)
                        TestParameters(constructor, parameters, type.ToString());
                }
                else
                {
                    _failedAssertions.AddRange(failedAssertionsForCurrentConstructor);
                }
            }
        }

        private BindingFlags GetBindingFlags()
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            if (_testConfig.TestInternals)
                bindingFlags = bindingFlags | BindingFlags.NonPublic;

            return bindingFlags;
        }

        private object[] GetParameters(ConstructorInfo constructor)
        {
            var parameters = new List<object>();

            foreach (var parameterType in constructor.GetParameters().Select(x => x.ParameterType))
            {
                if (_objectBuilder.CanBuildObject(parameterType))
                {
                    parameters.Add(_objectBuilder.BuildObject(parameterType));
                }
                else
                {
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
                    try
                    {
                        var kackCode = false;

                        foreach (var constructor2 in parameterType.GetConstructors())
                        {
                            if (ProducesDependencyCircle(constructor2.GetParameters().Select(x => x.ParameterType)))
                            {
                                //
                            }
                            else
                            {
                                _foundImplementation.Add(parameterType);

                                var constructorParameters = GetParameters(constructor2);

                                if (constructorParameters != null)
                                {
                                    kackCode = true;
                                    parameters.Add(constructor2.Invoke(constructorParameters));
                                    break;
                                }
                            }
                        }

                        if (!kackCode)
                        {
                            _failedAssertions.Add(string.Format("Class {0} makes trouble: cannot find an implementation of parameter {1} of constructor {2}.",
                                constructor.DeclaringType, 1, constructor));
                            return null;
                        }
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
            }

            return parameters.ToArray();
        }

        private object SearchImplementation(Type internalAbstractType, ConstructorInfo parentConstructor)
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
                                parametersForConstructor = GetParameters(constructor);
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

            _failedAssertions.Add(string.Format("Class {0} makes trouble: cannot find an implementation of parameter {1} of constructor {2}.",
                                parentConstructor.DeclaringType, 1, parentConstructor));

            return null;
        }

        private bool ProducesDependencyCircle(IEnumerable<Type> parameters)
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

        private void TestParameters(ConstructorInfo constructor, object[] parameters, string classUnderTest)
        {
            for (int parameterCounter = 0; parameterCounter < parameters.Length; parameterCounter++)
            {
                var parameterType = constructor.GetParameters()[parameterCounter].ParameterType;

                if (!parameterType.IsValueType ||
                    (_testConfig.TestNullables && Nullable.GetUnderlyingType(parameterType) != null))
                    TestOneParameterForNull(constructor, parameters, parameterCounter, classUnderTest);
            }
        }

        private void TestOneParameterForNull(ConstructorInfo constructor, object[] parameters, int parameterToTest, string classUnderTest)
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
                _failedAssertions.Add(string.Format("Class {0} makes trouble: parameter {1} of constructor {2} was not tested for null.",
                    classUnderTest,
                    parameterToTest + 1,
                    constructor));
        }
    }
}