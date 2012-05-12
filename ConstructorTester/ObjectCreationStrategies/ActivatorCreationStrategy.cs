using System;
using System.Collections.Generic;
using System.Reflection;
using ConstructorTester.Constraints;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class ActivatorCreationStrategy : ObjectCreationStrategyBase
    {
        private readonly ObjectBuilder _objectBuilder;
        private readonly ConstraintsTester _constraintsTester;
        private readonly ArgumentsForConstructors _argumentsForConstructors;
        private readonly TestConfig _testConfig;

        public ActivatorCreationStrategy(ObjectBuilder objectBuilder, ConstraintsTester constraintsTester, ArgumentsForConstructors argumentsForConstructors, TestConfig testConfig)
        {
            _objectBuilder = objectBuilder;
            _constraintsTester = constraintsTester;
            _argumentsForConstructors = argumentsForConstructors;
            _testConfig = testConfig;
        }

        public override bool CanCreate(Type type)
        {
            return type.IsClass &&
                   !type.IsAbstract &&
                   type.GetConstructors().Length > 0 &&
                   !_testConfig.TypesNotToTest.Contains(type) &&
                   CanBuildAllConstructorParameters(type);
        }

        private bool CanBuildAllConstructorParameters(Type type)
        {
            foreach (ConstructorInfo constructorInfo in type.GetConstructors())
            {
                var canBuild = true;

                if (!_argumentsForConstructors.AreAvailableFor(constructorInfo))
                {
                    foreach (ParameterInfo parameterInfo in constructorInfo.GetParameters())
                    {
                        if (_constraintsTester.ViolatesConstraints(parameterInfo.ParameterType) ||
                            !_objectBuilder.CanBuildObject(parameterInfo.ParameterType))
                        {
                            canBuild = false;
                            break;
                        }
                    }
                }

                if (canBuild)
                    return true;
            }

            return false;
        }

        public override object Create(Type type)
        {
            foreach (var constructorInfo in type.GetConstructors())
            {
                var parameters = new List<object>();

                if (_argumentsForConstructors.AreAvailableFor(constructorInfo))
                {
                    parameters.AddRange(_argumentsForConstructors.GetArguments(constructorInfo));
                }
                else
                {
                    foreach (var parameterInfo in constructorInfo.GetParameters())
                        parameters.Add(_objectBuilder.BuildObject(parameterInfo.ParameterType));
                }

                return constructorInfo.Invoke(parameters.ToArray());
            }

            return null;
        }
    }
}