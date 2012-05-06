using System;
using System.Collections.Generic;
using System.Linq;
using ConstructorTester.Constraints;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal class SearchForAnImplementationCreationStrategy : ObjectCreationStrategyBase
    {
        private readonly ObjectBuilder _objectBuilder;
        private readonly ConstraintsTester _constraintsTester;
        private readonly List<Type> _alreadyCreatedTypes = new List<Type>();

        public SearchForAnImplementationCreationStrategy(ObjectBuilder objectBuilder, ConstraintsTester constraintsTester)
        {
            _objectBuilder = objectBuilder;
            _constraintsTester = constraintsTester;
        }

        public override bool CanCreate(Type type)
        {
            return FindImplementationsFor(type)
                .Where(t => !_alreadyCreatedTypes.Contains(t))
                .Any(t => !_constraintsTester.ViolatesConstraints(t) && _objectBuilder.CanBuildObject(t));
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
        }
    }
}