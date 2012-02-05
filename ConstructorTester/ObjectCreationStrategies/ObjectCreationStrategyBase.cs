using System;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal abstract class ObjectCreationStrategyBase : IObjectCreationStrategy
    {
        public abstract bool CanCreate(Type type);
        public abstract object Create(Type type);

        public virtual void Reset() { }
    }
}