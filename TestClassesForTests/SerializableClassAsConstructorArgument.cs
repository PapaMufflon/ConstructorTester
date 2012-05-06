using System;

namespace TestClassesForTests
{
    public class SerializableClassAsConstructorArgument
    {
        public SerializableClassAsConstructorArgument(SerializableClass serializableClass)
        {
        }
    }

    public class Foo
    {
        public Foo(Bar bar)
        {
            
        }
    }

    [Serializable]
    public class Bar
    {
        
    }
}