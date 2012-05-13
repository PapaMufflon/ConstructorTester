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
            return type.GetConstructors().Any(CanBuildConstructor);
        }

        private bool CanBuildConstructor(ConstructorInfo constructorInfo)
        {
            var canBuild = false;

            if (_argumentsForConstructors.AreAvailableFor(constructorInfo))
                canBuild = true;
            else
                canBuild = constructorInfo.GetParameters().All(CanBuildParameter);

            return canBuild;
        }

        private bool CanBuildParameter(ParameterInfo parameterInfo)
        {
            if (_alreadyLookedForTypes.Contains(parameterInfo.ParameterType))
                return false;

            _alreadyLookedForTypes.Add(parameterInfo.ParameterType);

            if (!_objectBuilder.CanBuildObject(parameterInfo.ParameterType))
                return false;

            _alreadyLookedForTypes.Remove(parameterInfo.ParameterType);

            return true;
        }

        public override object Create(Type type)
        {
            _typesAlreadyTriedToCreate.Add(type);

            foreach (var constructorInfo in type.GetConstructors())
            {
                object[] parameters;

                if (!TryGetParameters(constructorInfo, out parameters))
                    continue;

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

        private bool TryGetParameters(ConstructorInfo constructorInfo, out object[] parameters)
        {
            parameters = null;
            var tempParameters = new List<object>();

            if (_argumentsForConstructors.AreAvailableFor(constructorInfo))
            {
                tempParameters.AddRange(_argumentsForConstructors.GetArguments(constructorInfo));
            }
            else
            {
                var parameterTypes = constructorInfo
                    .GetParameters()
                    .Select(x => x.ParameterType)
                    .ToList();

                if (CanBuildAllParameters(parameterTypes))
                    tempParameters.AddRange(parameterTypes.Select(parameterInfo => _objectBuilder.BuildObject(parameterInfo)));
            }

            parameters = tempParameters.ToArray();
            return constructorInfo.GetParameters().Length == 0 || parameters.Length > 0;
        }

        private bool CanBuildAllParameters(IEnumerable<Type> parameterTypes)
        {
            return parameterTypes.All(x => !_typesAlreadyTriedToCreate.Contains(x) &&
                                           _objectBuilder.CanBuildObject(x));
        }

        public override void Reset()
        {
            _typesAlreadyTriedToCreate.Clear();
            _alreadyLookedForTypes.Clear();
        }
    }
}