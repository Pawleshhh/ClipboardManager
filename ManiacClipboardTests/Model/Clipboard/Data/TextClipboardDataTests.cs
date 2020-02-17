using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManiacClipboard.Model.Tests
{
    [TestClass()]
    public class TextClipboardDataTests
    {
        [TestMethod]
        public void IEquatableEquals_CheckingEqualityWhenParameterIsNull_ReturnsFalse()
        {
            var clipboardData = new TextClipboardData("text");

            bool result = clipboardData.Equals(null);

            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("data", "data", true)]
        [DataRow("data1", "data2", false)]
        public void IEquatableEquals_CheckingEquality_ReturnsExpectedValue(string data1, string data2, bool expected)
        {
            var clipboardData1 = new TextClipboardData(data1);
            var clipboardData2 = new TextClipboardData(data2);

            bool result = clipboardData1.Equals(clipboardData2);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Equals_CheckingEqualityWhenParameterIsNull_ReturnsFalse()
        {
            object clipboardData = new TextClipboardData("text");

            bool result = clipboardData.Equals(null);

            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("data", "data", true)]
        [DataRow("data1", "data2", false)]
        public void Equals_CheckingEquality_ReturnsExpectedValue(string data1, string data2, bool expected)
        {
            object clipboardData1 = new TextClipboardData(data1);
            object clipboardData2 = new TextClipboardData(data2);

            bool result = clipboardData1.Equals(clipboardData2);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Equals_CheckingEqualitWhenObjectsAreSame_ReturnsTrue()
        {
            object clipboardData1 = new TextClipboardData("data");
            object clipboardData2 = clipboardData1;

            bool result = clipboardData1.Equals(clipboardData2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_CheckingEqualityWhenObjectsAreNotTheSameType_ReturnsFalse()
        {
            object clipboardData1 = new TextClipboardData("data");
            object clipboardData2 = new object();

            bool result = clipboardData1.Equals(clipboardData2);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetHashCode_ReturnsExpectedHashCode()
        {
            int expectedHashCode = "data".GetHashCode() * 17;
            var clipboardData = new TextClipboardData("data");

            int result = clipboardData.GetHashCode();

            Assert.AreEqual(expectedHashCode, result);
        }
    }
}