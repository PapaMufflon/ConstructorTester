using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConstructorTester
{
    internal class ArgumentsForConstructors
    {
        private readonly Dictionary<ConstructorInfo, object[]> _constructorArguments = new Dictionary<ConstructorInfo, object[]>();

        public void Add(Type type, object[] parameters)
        {
            foreach (var constructorInfo in type.GetConstructors())
            {
                var constructorParameters = constructorInfo.GetParameters();

                if (constructorParameters.Length != parameters.Length)
                    continue;

                if (Enumerable.Range(0, parameters.Count()).Any(index => constructorParameters[index].ParameterType != parameters[index].GetType()))
                    continue;

                if (_constructorArguments.ContainsKey(constructorInfo))
                    _constructorArguments.Remove(constructorInfo);

                _constructorArguments.Add(constructorInfo, parameters);
                return;
            }
        }

        public bool AreAvailableFor(ConstructorInfo constructorInfo)
        {
            return GetArguments(constructorInfo) != null;
        }

        public IEnumerable<object> GetArguments(ConstructorInfo constructorInfo)
        {
            if (!_constructorArguments.ContainsKey(constructorInfo))
                return null;

            return _constructorArguments[constructorInfo];
        }
    }
}