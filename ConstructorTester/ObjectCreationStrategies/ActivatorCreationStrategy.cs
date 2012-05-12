using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class ActivatorCreationStrategy : ObjectCreationStrategyBase
    {
        private readonly ObjectBuilder _objectBuilder;
        private readonly ArgumentsForConstructors _argumentsForConstructors;
        private readonly TestConfig _testConfig;
        private readonly List<Type> _typesAlreadyTriedToCreate = new List<Type>();
        private readonly List<Type> _alreadyLookedForTypes = new List<Type>();

        public ActivatorCreationStrategy(ObjectBuilder objectBuilder, ArgumentsForConstructors argumentsForConstructors, TestConfig testConfig)
        {
            _objectBuilder = objectBuilder;
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
                        if (_alreadyLookedForTypes.Contains(parameterInfo.ParameterType))
                        {
                            canBuild = false;
                            break;
                        }

                        _alreadyLookedForTypes.Add(parameterInfo.ParameterType);

                        if (!_objectBuilder.CanBuildObject(parameterInfo.ParameterType))
                        {
                            canBuild = false;
                            break;
                        }

                        _alreadyLookedForTypes.Remove(parameterInfo.ParameterType);
                    }
                }

                if (canBuild)
                    return true;
            }

            return false;
        }

        public override object Create(Type type)
        {
            _typesAlreadyTriedToCreate.Add(type);

            foreach (var constructorInfo in type.GetConstructors())
            {
                var parameters = new List<object>();

                if (_argumentsForConstructors.AreAvailableFor(constructorInfo))
                {
                    parameters.AddRange(_argumentsForConstructors.GetArguments(constructorInfo));
                }
                else
                {
                    var parameterTypes = constructorInfo.GetParameters().Select(x => x.ParameterType);

                    if (parameterTypes.All(x => !_typesAlreadyTriedToCreate.Contains(x) && _objectBuilder.CanBuildObject(x)))
                        parameters.AddRange(parameterTypes.Select(parameterInfo => _objectBuilder.BuildObject(parameterInfo)));
                    else
                        continue;
                }

                try
                {
                    return constructorInfo.Invoke(parameters.ToArray());
                }
                catch (TargetInvocationException)
                {
                    // continue
                }
            }

            return null;
        }

        public override void Reset()
        {
            _typesAlreadyTriedToCreate.Clear();
            _alreadyLookedForTypes.Clear();
        }
    }
}