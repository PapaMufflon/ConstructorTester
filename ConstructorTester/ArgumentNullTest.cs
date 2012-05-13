using System;
using System.Collections.Generic;
using System.IO;
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
        internal TestConfig TestConfig { get; private set; }

        private static ArgumentNullTest _argumentNullTest;
        private static readonly ArgumentsForConstructors ArgumentsForConstructors = new ArgumentsForConstructors();
        private readonly TypeTester _typeTester;

        private ArgumentNullTest(TypeTester typeTester, TestConfig testConfig)
        {
            TestConfig = testConfig;
            _typeTester = typeTester;
        }

        /// <summary>
        /// If set to <c>true</c>, <c>Nullable&lt;T&gt;</c> should also be checked for not null.
        /// </summary>
        public static bool TestNullables
        {
            get { return Instance.TestConfig.TestNullables; }
            set { Instance.TestConfig.TestNullables = value; }
        }

        /// <summary>
        /// If set to <c>true</c>, internal constructors are tested also
        /// (please make sure, ConstructorTester has access to internal classes via the InternalsVisibleTo-attribute).
        /// </summary>
        public static bool TestInternals
        {
            get { return Instance.TestConfig.TestInternals; }
            set { Instance.TestConfig.TestInternals = value; }
        }

        private static ArgumentNullTest Instance
        {
            get
            {
                if (_argumentNullTest == null)
                    CreateArgumentNullTest();

                return _argumentNullTest;
            }
        }

        private static void CreateArgumentNullTest()
        {
            var objectBuilder = new ObjectBuilder();
            objectBuilder.ObjectCreationStrategies.AddRange(BuildObjectCreationStrategies());

            var testConfig = new TestConfig(objectBuilder);
            objectBuilder.ObjectCreationStrategies.Add(new MockObjectCreationStrategy(objectBuilder));
            objectBuilder.ObjectCreationStrategies.Add(new RegisteredImplementationCreationStrategy(testConfig));
            objectBuilder.ObjectCreationStrategies.Add(new SearchForAnImplementationCreationStrategy(objectBuilder));
            objectBuilder.ObjectCreationStrategies.Add(new ActivatorCreationStrategy(objectBuilder, ArgumentsForConstructors, testConfig));

            var constraintsTester = new ConstraintsTester(BuildConstraints(testConfig), testConfig);
            var typeTester = new TypeTester(testConfig, constraintsTester, objectBuilder);

            _argumentNullTest = new ArgumentNullTest(typeTester, testConfig);
        }

        private static IEnumerable<IObjectCreationStrategy> BuildObjectCreationStrategies()
        {
            return new List<IObjectCreationStrategy>
            {
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
        /// Tests all classes specified by the path and searchPattern.
        /// </summary>
        /// <param name="path">The directory to search.</param>
        /// <param name="searchPattern">The search string to match against the names of directories in path.</param>
        /// <param name="searchOption">One of the values of the SearchOption enumeration that specifies whether the search operation should include only the current directory or should include all subdirectories.The default value is TopDirectoryOnly.</param> 
        public static void Execute(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var types = new List<Type>();

            foreach (var file in Directory.EnumerateFiles(path, searchPattern, searchOption))
            {
                try
                {
                    types.AddRange(Assembly.LoadFrom(file).GetTypes());
                }
                catch (BadImageFormatException e)
                {
                    throw new ArgumentException("Provide a searchPattern that finds only valid assemblies.", e);
                }
            }

            Instance.TestTypes(types);
        }

        /// <summary>
        /// Tests all classes in an assembly and all constructors within these classes
        /// if all parameters are checked for null.
        /// </summary>
        /// <param name="assemblyUnderTest"><c>Assembly</c> to check.</param>
        public static void Execute(Assembly assemblyUnderTest)
        {
            Instance.TestTypes(assemblyUnderTest.GetTypes());
        }

        /// <summary>
        /// Tests all constructors of the given <c>Type</c> if all parameters are checked for null.
        /// </summary>
        /// <param name="classUnderTest"><c>Type</c> to check.</param>
        public static void Execute(Type classUnderTest)
        {
            Instance.TestTypes(new List<Type> { classUnderTest });
        }

        private void TestTypes(IEnumerable<Type> types)
        {
            var results = new List<string>();

            foreach (var type in types.Where(x => !Instance.TestConfig.TypesNotToTest.Contains(x)))
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
            Instance.TestConfig.RegisterImplementationFor(typeof(TBase), typeof(TImplementation));
        }

        /// <summary>
        /// Registers a concrete object to be used for an abstract type or interface.
        /// </summary>
        /// <typeparam name="T">The abstract type or interface to be substituted.</typeparam>
        /// <param name="implementation">The concrete object which should be used for <c>T</c>.</param>
        public static void Register<T>(T implementation)
        {
            Instance.TestConfig.RegisterImplementationFor(typeof(T), implementation);
        }

        /// <summary>
        /// Empties all registrations made via <c>Register</c> and resets the types not to test.
        /// </summary>
        public static void Reset()
        {
            Instance.TestConfig.ClearImplementations();
            Instance.TestConfig.TypesNotToTest.Clear();
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

            Instance.TestConfig.TypesNotToTest.Add(type);
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