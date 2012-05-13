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
        private readonly ConstraintsTester _constraintsTester;
        private readonly ObjectBuilder _objectBuilder;
        private readonly List<string> _failedAssertions = new List<string>();

        public TypeTester(TestConfig testConfig, ConstraintsTester constraintsTester, ObjectBuilder objectBuilder)
        {
            _testConfig = testConfig;
            _constraintsTester = constraintsTester;
            _objectBuilder = objectBuilder;
        }

        public IEnumerable<string> TestForNullArgumentExceptionsInConstructor(Type type)
        {
            _objectBuilder.Reset();

            _failedAssertions.Clear();
            _failedAssertions.AddRange(_constraintsTester.EvaluateConstraints(type));

            if (_failedAssertions.Count == 0)
                TestType(type);

            return _failedAssertions;
        }

        private void TestType(Type type)
        {
            var bindingFlags = _testConfig.GetBindingFlags();

            foreach (var constructor in type.GetConstructors(bindingFlags))
            {
                object[] parameters;
                var failedAssertionsForCurrentConstructor = _constraintsTester.EvaluateConstraints(constructor);

                if (!failedAssertionsForCurrentConstructor.Any() &&
                    TryGetParameters(constructor, out parameters))
                {
                    TestParameters(constructor, parameters, type.ToString());
                }
                else
                {
                    _failedAssertions.AddRange(failedAssertionsForCurrentConstructor);
                }
            }
        }

        private bool TryGetParameters(ConstructorInfo constructor, out object[] parameters)
        {
            var result = true;
            var tempParameters = new List<object>();
            var parameterCounter = 0;

            foreach (var parameterType in constructor.GetParameters().Select(x => x.ParameterType))
            {
                parameterCounter++;

                if (_objectBuilder.CanBuildObject(parameterType))
                {
                    tempParameters.Add(_objectBuilder.BuildObject(parameterType));
                }
                else
                {
                    _failedAssertions.Add(string.Format("There was a problem when testing class {0}: cannot find an implementation for parameter {1} of constructor {2}.",
                                constructor.DeclaringType, parameterCounter, constructor));

                    result = false;
                }
            }

            parameters = tempParameters.ToArray();

            return result;
        }

        private void TestParameters(ConstructorInfo constructor, object[] parameters, string classUnderTest)
        {
            for (int parameterCounter = 0; parameterCounter < parameters.Length; parameterCounter++)
            {
                var parameterType = constructor.GetParameters()[parameterCounter].ParameterType;

                if (CanTestParameterForNull(parameterType))
                    TestOneParameterForNull(constructor, parameters, parameterCounter, classUnderTest);
            }
        }

        private bool CanTestParameterForNull(Type parameterType)
        {
            return !parameterType.IsValueType ||
                   (_testConfig.TestNullables && Nullable.GetUnderlyingType(parameterType) != null);
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
                _failedAssertions.Add(string.Format("Found a weakness in class {0}: parameter {1} of constructor {2} was not tested for null.",
                    classUnderTest,
                    parameterToTest + 1,
                    constructor));
        }
    }
}