using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConstructorTester
{
    internal class TestConfig
    {
        public bool TestInternals { get; set; }
        public bool TestNullables { get; set; }
        public List<Type> TypesNotToTest { get; private set; }

        private readonly ObjectBuilder _objectBuilder;
        private readonly Dictionary<Type, object> _container = new Dictionary<Type, object>();

        public TestConfig(ObjectBuilder objectBuilder)
        {
            _objectBuilder = objectBuilder;

            TypesNotToTest = new List<Type>();
        }

        public bool HasImplementationFor(Type type)
        {
            return _container.ContainsKey(type);
        }

        public void ClearImplementations()
        {
            _container.Clear();
        }

        public void RegisterImplementationFor(Type type, object implementation)
        {
            if (_container.ContainsKey(type))
                _container.Remove(type);

            _container.Add(type, implementation);
        }

        public void RegisterImplementationFor(Type baseType, Type implementingType)
        {
            RegisterImplementationFor(baseType, _objectBuilder.BuildObject(implementingType));
        }

        public object GetImplementationFor(Type type)
        {
            if (!_container.ContainsKey(type))
                throw new ArgumentException("There is no implementation registered for type " + type);

            return _container[type];
        }

        public BindingFlags GetBindingFlags()
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            if (TestInternals)
                bindingFlags = bindingFlags | BindingFlags.NonPublic;

            return bindingFlags;
        }
    }
}