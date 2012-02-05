using System;

namespace ConstructorTester.ObjectCreationStrategies
{
    internal interface IObjectCreationStrategy
    {
        bool CanCreate(Type type);
        object Create(Type type);

        void Reset();
    }
}