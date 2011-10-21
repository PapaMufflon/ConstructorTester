using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rhino.Mocks;

namespace ConstructorTester
{
    public class ArgumentNullTest
    {
        private static Action<string> _failedAssertionAction;

        /// <summary>
        /// If a parameter is found, which is not checked for null, this action is invoked.
        /// You can set your Assertion here, for example:
        /// ArgumentNullTest.FailedAssertionAction = message => Assert.Fail(message);
        /// </summary>
        public static Action<string> FailedAssertionAction
        {
            private get
            {
                if (_failedAssertionAction == null)
                    _failedAssertionAction = s => Console.WriteLine(s);

                return _failedAssertionAction;
            }

            set
            {
                _failedAssertionAction = value;
            }
        }

        /// <summary>
        /// Tests all classes in an assembly and all constructors within these classes
        /// if all parameters are checked for null.
        /// </summary>
        /// <param name="assemblyUnderTest"><c>Assembly</c> to check.</param>
        public static void Execute(Assembly assemblyUnderTest)
        {
            foreach (var classUnderTest in assemblyUnderTest.GetTypes())
                Execute(classUnderTest);
        }

        /// <summary>
        /// Tests all constructors of the given <c>Type</c> if all parameters are checked for null.
        /// </summary>
        /// <param name="classUnderTest"><c>Type</c> to check.</param>
        public static void Execute(Type classUnderTest)
        {
            if (!classUnderTest.IsAbstract)
            {
                foreach (var constructor in classUnderTest.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                {
                    var parameters = GetParameters(constructor.GetParameters());
                    TestParameters(constructor, parameters, classUnderTest.ToString());
                }
            }
        }

        private static object[] GetParameters(ParameterInfo[] parameterInfo)
        {
            var parameters = new List<object>();

            foreach (var parameterType in parameterInfo.Select(x => x.ParameterType))
            {
                if (parameterType.IsInterface)
                {
                    parameters.Add(MockRepository.GenerateStub(parameterType));
                }
                else if (parameterType.IsValueType)
                {
                    parameters.Add(Activator.CreateInstance(parameterType));
                }
                else if (parameterType == typeof(string))
                {
                    parameters.Add("");
                }
                else
                {
                    var constructors = parameterType.GetConstructors();

                    var constructor = parameterType.GetConstructors()[0];
                    var parametersForConstructor = GetParameters(constructor.GetParameters());
                    parameters.Add(constructor.Invoke(parametersForConstructor));
                }
            }

            return parameters.ToArray();
        }

        private static void TestParameters(ConstructorInfo constructor, object[] parameters, string classUnderTest)
        {
            for (int parameterCounter = 0; parameterCounter < parameters.Length; parameterCounter++)
            {
                if (!parameters[parameterCounter].GetType().IsValueType)
                    TestOneParameterForNull(constructor, parameters, parameterCounter, classUnderTest);
            }
        }

        private static void TestOneParameterForNull(ConstructorInfo constructor, object[] parameters, int parameterToTest, string classUnderTest)
        {
            // copy parameters to not destroy them
            var parametersCopy = (object[])parameters.Clone();
            parametersCopy[parameterToTest] = null;

            var catched = false;

            try
            {
                constructor.Invoke(parametersCopy);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null &&
                    e.InnerException.GetType() == typeof(ArgumentNullException))
                    catched = true;
            }

            if (!catched)
                FailedAssertionAction.Invoke(string.Format("Parameter number {0} of constructor {1} of class {2} was not tested for null.", parameterToTest, constructor, classUnderTest));
        }
    }
}
