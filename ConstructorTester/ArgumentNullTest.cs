using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConstructorTester.Constraints;
using ConstructorTester.ObjectCreationStrategies;

namespace ConstructorTester
{
    /// <summary>
    /// Checks classes if they throw <c>ArgumentNullException</c>s for their constructor arguments when they are null.
    /// </summary>
    public class ArgumentNullTest
    {
        /// <summary>
        /// If set to <c>true</c>, <c>Nullable&lt;T&gt;</c> should also be checked for not null.
        /// </summary>
        public static bool TestNullables
        {
            get { return Current.TestConfig.TestNullables; }
            set { Current.TestConfig.TestNullables = value; }
        }

        /// <summary>
        /// If set to <c>true</c>, internal constructors are tested also
        /// (please make sure, ConstructorTester has access to internal classes via the InternalsVisibleTo-attribute).
        /// </summary>
        public static bool TestInternals
        {
            get { return Current.TestConfig.TestInternals; }
            set { Current.TestConfig.TestInternals = value; }
        }

        internal TestConfig TestConfig { get; private set; }

        private static ArgumentNullTest _argumentNullTest;
        private static readonly ArgumentsForConstructors ArgumentsForConstructors = new ArgumentsForConstructors();
        private readonly TypeTester _typeTester;

        private ArgumentNullTest(TypeTester typeTester, TestConfig testConfig)
        {
            TestConfig = testConfig;
            _typeTester = typeTester;
        }

        private static ArgumentNullTest Current
        {
            get
            {
                if (_argumentNullTest == null)
                    CreateCurrentArgumentNullTest();

                return _argumentNullTest;
            }
        }

        private static void CreateCurrentArgumentNullTest()
        {
            var objectBuilder = new ObjectBuilder();
            objectBuilder.ObjectCreationStrategies.AddRange(BuildObjectCreationStrategies());

            var testConfig = new TestConfig(objectBuilder);
            var constraintsTester = new ConstraintsTester(BuildConstraints(testConfig), testConfig);

            objectBuilder.ObjectCreationStrategies.Add(new RegisteredImplementationCreationStrategy(testConfig));
            objectBuilder.ObjectCreationStrategies.Add(new SearchForAnImplementationCreationStrategy(objectBuilder));
            objectBuilder.ObjectCreationStrategies.Add(new ActivatorCreationStrategy(objectBuilder, ArgumentsForConstructors, testConfig));

            var typeTester = new TypeTester(testConfig, constraintsTester, objectBuilder);
            _argumentNullTest = new ArgumentNullTest(typeTester, testConfig);
        }

        private static IEnumerable<IObjectCreationStrategy> BuildObjectCreationStrategies()
        {
            return new List<IObjectCreationStrategy>
            {
                new MockObjectCreationStrategy(),
                new EnumValueCreationStrategy(),
                new ValueTypeCreationStrategy(),
                new StructCreationStrategy(),
                new NullableObjectCreationStrategy(),
                new StringCreationStrategy(),
                new GenericObjectCreationStrategy(),
                new DelegateCreationStrategy()
            };
        }

        private static IEnumerable<IConstraint> BuildConstraints(TestConfig testConfig)
        {
            return new List<IConstraint>
            {
                new CannotTestNonPublicTypesWhenTestInternalsIsFalse(testConfig),
                new CannotTestAbstractClass(),
                new CannotTestGenericConstructors(),
                new CannotTestConstructorsWithPointerArguments(),
                new CannotTestConstructorsWithRuntimeArgumentHandleArguments(),
                new CannotTestConstructorsWithByRefArguments()
            };
        }

        /// <summary>
        /// Tests all classes in an assembly and all constructors within these classes
        /// if all parameters are checked for null.
        /// </summary>
        /// <param name="assemblyUnderTest"><c>Assembly</c> to check.</param>
        public static void Execute(Assembly assemblyUnderTest)
        {
            Current.TestTypes(assemblyUnderTest.GetTypes());
        }

        /// <summary>
        /// Tests all constructors of the given <c>Type</c> if all parameters are checked for null.
        /// </summary>
        /// <param name="classUnderTest"><c>Type</c> to check.</param>
        public static void Execute(Type classUnderTest)
        {
            Current.TestTypes(new List<Type> { classUnderTest });
        }

        private void TestTypes(IEnumerable<Type> types)
        {
            var results = new List<string>();

            foreach (var type in types.Where(x => !Current.TestConfig.TypesNotToTest.Contains(x)))
            {
                results.AddRange(_typeTester.TestForNullArgumentExceptionsInConstructor(type));
            }

            EvaluateResult(results);
        }

        private static void EvaluateResult(ICollection<string> failedAssertions)
        {
            if (failedAssertions.Count > 0)
                throw new ArgumentException(failedAssertions.Aggregate("", (s, t) => s + Environment.NewLine + t).Substring(Environment.NewLine.Length));
        }

        /// <summary>
        /// Registers a concrete type to be used for an abstract type or interface.
        /// </summary>
        /// <typeparam name="TBase">The abstract type or interface to be substituted.</typeparam>
        /// <typeparam name="TImplementation">The concrete type which should be used for <c>TBase</c>.</typeparam>
        public static void Register<TBase, TImplementation>()
            where TImplementation : TBase
            where TBase : class
        {
            Current.TestConfig.RegisterImplementationFor(typeof(TBase), typeof(TImplementation));
        }

        /// <summary>
        /// Registers a concrete object to be used for an abstract type or interface.
        /// </summary>
        /// <typeparam name="T">The abstract type or interface to be substituted.</typeparam>
        /// <param name="implementation">The concrete object which should be used for <c>T</c>.</param>
        public static void Register<T>(T implementation)
        {
            Current.TestConfig.RegisterImplementationFor(typeof(T), implementation);
        }

        /// <summary>
        /// Empties all registrations made via <c>Register</c> and resets the types not to test.
        /// </summary>
        public static void Reset()
        {
            Current.TestConfig.ClearImplementations();
            Current.TestConfig.TypesNotToTest.Clear();
        }

        /// <summary>
        /// Exclude given type in all the tests.
        /// </summary>
        /// <param name="type">Type to leave out for testing.</param>
        /// <param name="reason">Give a reason why not to test this type.</param>
        public static void Exclude(Type type, string reason)
        {
            if (string.IsNullOrEmpty(reason))
                throw new ArgumentNullException("reason", "Don't be lazy - give me a reason!");

            Current.TestConfig.TypesNotToTest.Add(type);
        }

        /// <summary>
        /// Use given parameters for the constructor of the given type.
        /// </summary>
        /// <typeparam name="T">The type to use the parameters for.</typeparam>
        /// <param name="parameters">The parameters to use for the given type.</param>
        public static void UseFollowingConstructorParameters<T>(params object[] parameters)
        {
            ArgumentsForConstructors.Add(typeof(T), parameters);
        }
    }
}