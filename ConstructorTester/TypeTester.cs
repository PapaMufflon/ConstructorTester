using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConstructorTester.Constraints;

namespace ConstructorTester
{
    internal class TypeTester
    {
        private readonly TestConfig _testConfig;
        private readonly List<IConstraint> _constraints;
        private readonly ObjectBuilder _objectBuilder;
        private readonly List<string> _failedAssertions = new List<string>();

        public TypeTester(TestConfig testConfig, List<IConstraint> constraints, ObjectBuilder objectBuilder)
        {
            _testConfig = testConfig;
            _constraints = constraints;
            _objectBuilder = objectBuilder;
        }

        public IEnumerable<string> TestForNullArgumentExceptionsInConstructor(Type type)
        {
            _objectBuilder.Reset();

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
            var parameterCounter = 0;

            foreach (var parameterType in constructor.GetParameters().Select(x => x.ParameterType))
            {
                parameterCounter++;

                if (_objectBuilder.CanBuildObject(parameterType))
                {
                    parameters.Add(_objectBuilder.BuildObject(parameterType));
                }
                else
                {
                    _failedAssertions.Add(string.Format("Class {0} makes trouble: cannot find an implementation of parameter {1} of constructor {2}.",
                                constructor.DeclaringType, parameterCounter, constructor));
                }
            }

            return parameters.ToArray();
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

            if (!catched)
                _failedAssertions.Add(string.Format("Class {0} makes trouble: parameter {1} of constructor {2} was not tested for null.",
                    classUnderTest,
                    parameterToTest + 1,
                    constructor));
        }
    }
}