using System;
using System.Collections.Generic;
using System.Linq;
using ConstructorTester.ObjectCreationStrategies;

namespace ConstructorTester
{
    internal class ObjectBuilder
    {
        public List<IObjectCreationStrategy> ObjectCreationStrategies { get; private set; }

        public ObjectBuilder()
        {
            ObjectCreationStrategies = new List<IObjectCreationStrategy>();
        }

        public object BuildObject(Type type)
        {
            return (from objectCreationStrategy in ObjectCreationStrategies
                    where objectCreationStrategy.CanCreate(type)
                    select objectCreationStrategy.Create(type)).FirstOrDefault();
        }

        /// <summary>
        /// Just to be able to refactor TypeTester.GetParameters in a TDD-manner. Delete this afterwards.
        /// </summary>
        public bool CanBuildObject(Type type)
        {
            return ObjectCreationStrategies.Any(x => x.CanCreate(type));
        }
    }
}