using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConstructorTester.Constraints;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class ActivatorCreationStrategy : ObjectCreationStrategyBase
    {
        private readonly ObjectBuilder _objectBuilder;
        private readonly ConstraintsTester _constraintsTester;

        public ActivatorCreationStrategy(ObjectBuilder objectBuilder, ConstraintsTester constraintsTester)
        {
            _objectBuilder = objectBuilder;
            _constraintsTester = constraintsTester;
        }

        public override bool CanCreate(Type type)
        {
            return type.IsClass &&
                !type.IsAbstract &&
                type.GetConstructors().Length > 0 &&
                CanBuildAllConstructorParameters(type);
        }

        private bool CanBuildAllConstructorParameters(Type type)
        {
            foreach (ConstructorInfo constructorInfo in type.GetConstructors())
            {
                var canBuild = true;

                foreach (ParameterInfo parameterInfo in constructorInfo.GetParameters())
                {
                    if (_constraintsTester.ViolatesConstraints(parameterInfo.ParameterType) ||
                        !_objectBuilder.CanBuildObject(parameterInfo.ParameterType))
                    {
                        canBuild = false;
                        break;
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

                foreach (var parameterInfo in constructorInfo.GetParameters())
                {
                    parameters.Add(_objectBuilder.BuildObject(parameterInfo.ParameterType));
                }

                return constructorInfo.Invoke(parameters.ToArray());
            }

            return null;
        }
    }
}