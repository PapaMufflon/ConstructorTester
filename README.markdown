# Constructor Tester

Tests constructors in a given assembly or class for ArgumentNullExceptions.

## Usage (in an NUnit test for example)

    [TestFixture]
    public class DemoClassTests
    {
        [Test]
        public void Demo()
        {
            ArgumentNullTest.Execute(typeof(DemoClass).Assembly);
        }
    }

    public class DemoClass
    {
        public DemoClass(Object o)
        {
        }
    }

Tests all constructors in the assembly of class DemoClass and produces following output:

    System.ArgumentException : Found a weakness in class ConstructorTesterTest.DemoClass: parameter 1 of constructor Void .ctor(System.Object) was not tested for null.

## API

Execute(Assembly assemblyUnderTest)
Tests all constructors in the given assembly.

Exectue(Class classUnderTest)
Tests all constructors in the given class.

TestInternals
If set to true, all internal constructors will be tested also. Make sure, you set the InternalsVisibleToAttribute for ConstructorTester.dll.

TestNullables
If set to true, Nullables will be tested also.

Register<TBase, TImplementation>()
Register<T>(T implementation)
If there is no implementation for an interface or abstract argument in a constructor, you can register one.

DeregisterEverything()
Resets all registered implementations.

DoNotIncludeInNextTest(Type type, string reason)
Maybe you have some really good excuses not to test an argument for null. Just exclude it giving a reason.

## Limitations
Right now, ConstructorTester cannot test all constructor-arguments. Following situations cannot be tested yet:
* Abstract classes
* Constructors with ByRef arguments
* Constructors with pointer arguments
* Constructors with runtime arguments
* Generic constructors