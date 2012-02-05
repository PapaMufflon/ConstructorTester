using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class SearchForAnImplementationCreationStrategy : IObjectCreationStrategy
    {
        private readonly ObjectBuilder _objectBuilder;
        private readonly List<Type> _foundImplementation = new List<Type>();

        public SearchForAnImplementationCreationStrategy(ObjectBuilder objectBuilder)
        {
            _objectBuilder = objectBuilder;
        }

        public bool CanCreate(Type type)
        {
            return FindImplementationsFor(type).Any(t => _objectBuilder.CanBuildObject(t));
        }

        public object Create(Type type)
        {
            _foundImplementation.Add(type);

            return FindImplementationsFor(type)
                .Where(t => _objectBuilder.CanBuildObject(t))
                .Select(t => _objectBuilder.BuildObject(t))
                .First();
        }

        private IEnumerable<Type> FindImplementationsFor(Type type)
        {
            return type.Assembly.GetTypes().Where(t => t.BaseType == type && !t.IsAbstract && !t.IsInterface);
        }
    }
}