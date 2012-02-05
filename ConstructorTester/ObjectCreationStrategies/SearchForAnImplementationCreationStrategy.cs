using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class SearchForAnImplementationCreationStrategy : ObjectCreationStrategyBase
    {
        private readonly ObjectBuilder _objectBuilder;
        private readonly List<Type> _alreadyCreatedTypes = new List<Type>();

        public SearchForAnImplementationCreationStrategy(ObjectBuilder objectBuilder)
        {
            _objectBuilder = objectBuilder;
        }

        public override bool CanCreate(Type type)
        {
            return FindImplementationsFor(type)
                .Where(t => !_alreadyCreatedTypes.Contains(t))
                .Any(t => _objectBuilder.CanBuildObject(t));
        }

        public override object Create(Type type)
        {
            var foundType= FindImplementationsFor(type)
                .Where(t => !_alreadyCreatedTypes.Contains(t))
                .Where(t => _objectBuilder.CanBuildObject(t))
                .FirstOrDefault();

            _alreadyCreatedTypes.Add(foundType);

            return _objectBuilder.BuildObject(foundType);
        }

        private IEnumerable<Type> FindImplementationsFor(Type type)
        {
            return type
                .Assembly
                .GetTypes()
                .Where(t => t.BaseType == type &&
                            !t.IsAbstract &&
                            !t.IsInterface);
        }

        public override void Reset()
        {
            _alreadyCreatedTypes.Clear();
        }
    }
}