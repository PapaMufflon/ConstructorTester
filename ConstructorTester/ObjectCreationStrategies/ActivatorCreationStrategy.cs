using System;
using System.Collections.Generic;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class ActivatorCreationStrategy : ObjectCreationStrategyBase
    {
        private readonly ObjectBuilder _objectBuilder;

        public ActivatorCreationStrategy(ObjectBuilder objectBuilder)
        {
            _objectBuilder = objectBuilder;
        }

        public override bool CanCreate(Type type)
        {
            return type.IsClass && !type.IsAbstract && type.GetConstructors().Length > 0;
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