using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ManiacClipboard.Model.Tests
{
    /// <summary>
    /// Unit tests of the <see cref="ClipboardSource"/> class.
    /// </summary>
    [TestClass()]
    public class ClipboardSourceTests
    {
        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void OneParameterConstructor_NullOrEmptyAppName_ThrowsArgumentException(string wrongAppName)
        {
            Assert.ThrowsException<ArgumentException>(
                () => new ClipboardSource(wrongAppName));
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void TwoParametersConstructor_NullOrEmptyAppName_ThrowsArgumentException(string wrongAppName)
        {
            Assert.ThrowsException<ArgumentException>(
                () => new ClipboardSource(wrongAppName, "path"));
        }

        [TestMethod]
        public void IEquatableEquals_CheckingEqualityWhenParameterIsNull_ReturnsFalse()
        {
            var source = new ClipboardSource("appName");

            bool result = source.Equals(null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IEquatableEquals_CheckingEqualityWhenTwoObjectsAreEqual_ReturnsTrue()
        {
            var source1 = new ClipboardSource("appName");
            var source2 = new ClipboardSource("appName");

            bool result = source1.Equals(source2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IEquatableEquals_CheckingEqualityWhenTwoObjectsAreNotEqual_ReturnsFalse()
        {
            var source1 = new ClipboardSource("appName1");
            var source2 = new ClipboardSource("appName2");

            bool result = source1.Equals(source2);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_CheckingEqualityWhenParameterIsNull_ReturnsFalse()
        {
            object source = new ClipboardSource("appName");

            bool result = source.Equals(null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_CheckingEqualityWhenTwoObjectsAreSame_ReturnsTrue()
        {
            object source1 = new ClipboardSource("appName");
            object source2 = source1;

            bool result = source1.Equals(source2);

            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("appName", "appName", true)]
        [DataRow("appName1", "appName2", false)]
        public void Equals_CheckingEqualityWhenTwoObjectsAreEqualOrNot_ReturnsExpectedValue(string data1, string data2, bool expected)
        {
            object source1 = new ClipboardSource(data1);
            object source2 = new ClipboardSource(data2);

            bool result = source1.Equals(source2);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Equals_CheckingEqualityWhenTwoObjectsAreNotTheSameType_ReturnsFalse()
        {
            object source1 = new ClipboardSource("appName");
            object source2 = "appName";

            bool result = source1.Equals(source2);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ToString_ReturnsAppName()
        {
            var source = new ClipboardSource("appName");

            string result = source.ToString();

            StringAssert.Contains("appName", result);
        }

        [TestMethod]
        public void GetHashCode_GetsProperHashCode()
        {
            string parameter = "appName";
            var source = new ClipboardSource(parameter);

            int expected = parameter.GetHashCode() * 13;
            int result = source.GetHashCode();

            Assert.AreEqual(expected, result);
        }
    }
}