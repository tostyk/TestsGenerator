using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using TestsGenerator.Example;

namespace Tests
{
    [TestFixture]
    public class ExampleClassTests
    {
        private ExampleClass _exampleClass;
        private Mock<ITestInterface > _testInterface ;
        [SetUp]
        public void SetUp()
        {
            _testInterface  = new Mock<ITestInterface >();
            int number = default;
            _exampleClass = new ExampleClass(_testInterface .Object, number);
        }

        [Test]
        public void VoidMethodTest()
        {
            _exampleClass.VoidMethod();
            Assert.Fail("autogenerated");
        }

        [Test]
        public void IntParameterlessMethodTest()
        {
            int actual = _exampleClass.IntParameterlessMethod();
            int expected = default;
            Assert.That(actual, Is.EqualTo(expected));
            Assert.Fail("autogenerated");
        }

        [Test]
        public void StringMethodWithParametersTest()
        {
            int number = default;
            string s = default;
            ExampleClass exampleClass = default;
            string actual = _exampleClass.StringMethodWithParameters(number, s, exampleClass);
            string expected = default;
            Assert.That(actual, Is.EqualTo(expected));
            Assert.Fail("autogenerated");
        }

        [Test]
        public void DuplicateMethodTest()
        {
            int number = default;
            int actual = _exampleClass.DuplicateMethod(number);
            int expected = default;
            Assert.That(actual, Is.EqualTo(expected));
            Assert.Fail("autogenerated");
        }

        [Test]
        public void DuplicateMethod2Test()
        {
            double number = default;
            int actual = _exampleClass.DuplicateMethod(number);
            int expected = default;
            Assert.That(actual, Is.EqualTo(expected));
            Assert.Fail("autogenerated");
        }
    }
}