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
        private readonly List<IResult> _results = new List<IResult>();

        public TypeTester(TestConfig testConfig, ConstraintsTester constraintsTester, ObjectBuilder objectBuilder)
        {
            _testConfig = testConfig;
            _constraintsTester = constraintsTester;
            _objectBuilder = objectBuilder;
        }

        public IEnumerable<IResult> TestForNullArgumentExceptionsInConstructor(Type type)
        {
            _objectBuilder.Reset();

            _results.Clear();
            _results.AddRange(_constraintsTester.EvaluateConstraints(type));

            if (_results.Count == 0)
                TestType(type);

            return _results;
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
                    TestParameters(constructor, parameters, type);
                }
                else
                {
                    _results.AddRange(failedAssertionsForCurrentConstructor);
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
                    _results.Add(new Problem
                    {
                        Message = string.Format("{0}: cannot find an implementation for parameter {1} of constructor {2}",
                            constructor.DeclaringType, parameterCounter, constructor)
                    });

                    result = false;
                }
            }

            parameters = tempParameters.ToArray();

            return result;
        }

        private void TestParameters(ConstructorInfo constructor, object[] parameters, Type classUnderTest)
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

        private void TestOneParameterForNull(ConstructorInfo constructor, object[] parameters, int parameterToTest, Type classUnderTest)
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
                _results.Add(new Weakness
                {
                    Type = classUnderTest,
                    Constructor = constructor,
                    ParameterPosition = parameterToTest + 1
                });
        }
    }
}