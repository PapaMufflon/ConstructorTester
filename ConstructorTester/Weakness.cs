using System;
using System.Reflection;

namespace ConstructorTester
{
    internal class Weakness : IResult
    {
        public Type Type { get; set; }
        public ConstructorInfo Constructor { get; set; }
        public int ParameterPosition { get; set; }
    }
}