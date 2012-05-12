using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class SearchForAnImplementationCreationStrategy : ObjectCreationStrategyBase
    {
        private readonly ObjectBuilder _objectBuilder;
        private readonly List<Type> _alreadyCreatedTypes = new List<Type>();
        private readonly List<Type> _alreadyLookedForTypes = new List<Type>();

        public SearchForAnImplementationCreationStrategy(ObjectBuilder objectBuilder)
        {
            _objectBuilder = objectBuilder;
        }

        public override bool CanCreate(Type type)
        {
            var implementations = FindImplementationsFor(type)
                .Where(t => !_alreadyLookedForTypes.Contains(t));

            foreach (var implementation in implementations)
            {
                _alreadyLookedForTypes.Add(implementation);

                if (_objectBuilder.CanBuildObject(implementation))
                    return true;

                _alreadyLookedForTypes.Remove(implementation);
            }

            return false;
        }

        public override object Create(Type type)
        {
            var foundType= FindImplementationsFor(type)
                .Where(t => !_alreadyCreatedTypes.Contains(t))
                .FirstOrDefault(t => _objectBuilder.CanBuildObject(t));

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
            _alreadyLookedForTypes.Clear();
        }
    }
}