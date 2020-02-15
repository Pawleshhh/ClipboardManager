using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManiacClipboard.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManiacClipboard.Model.Tests
{
    [TestClass()]
    public class UnknownClipboardDataTests
    {

        #region Tests

        [TestMethod]
        public void GetFormats_GettingFormats_GetsExpectedFormat()
        {
            var clipboardData = GetUnknownClipboardDataAsInt(10);

            string[] result = clipboardData.GetFormats();

            CollectionAssert.AreEquivalent(new string[] { "number" }, result);
        }

        [TestMethod]
        public void Equals_CheckingEqualityWhenParameterIsNull_ReturnsFalse()
        {
            var clipboardData = GetUnknownClipboardDataAsInt(10);

            bool result = clipboardData.Equals(null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_CheckingEqualityWhenObjectsAreTheSame_ReturnsTrue()
        {
            var clipboardData1 = GetUnknownClipboardDataAsInt(10);
            var clipboardData2 = clipboardData1;

            bool result = clipboardData1.Equals(clipboardData2);

            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(10, 10, true)]
        [DataRow(323, 10, false)]
        public void Equals_CheckingEqualityWhenDataAreEqualOrNot_ReturnsExpectedValue(int data1, int data2, bool expected)
        {
            var clipboardData1 = GetUnknownClipboardDataAsInt(data1);
            var clipboardData2 = GetUnknownClipboardDataAsInt(data2);

            bool result = clipboardData1.Equals(clipboardData2);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Equals_CheckingEqualityWhenObjectsAreNotTheSameType_ReturnsFalse()
        {
            var clipboardData1 = GetUnknownClipboardDataAsInt(10);
            object clipboardData2 = new object();

            bool result = clipboardData1.Equals(clipboardData2);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetHashCode_GettingHashCode_ReturnsExpectedHashCode()
        {
            int expected = 10.GetHashCode() * 13 * 17;
            var clipboardData = GetUnknownClipboardDataAsInt(10);

            int result = clipboardData.GetHashCode();

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ToString_InvokingToStringMethod_GetsTypeName()
        {
            string expectedTypeName = 10.GetType().Name;
            var clipboardData = GetUnknownClipboardDataAsInt(10);

            string result = clipboardData.ToString();

            Assert.AreEqual(expectedTypeName, result);
        }

        [TestMethod]
        public void Dispose_DisposingData_DataIsDisposed()
        {
            var data = new FakeDisposableObject();
            var clipboardData = new UnknownClipboardData(data, new string[] { "fake" });

            clipboardData.Dispose();

            Assert.IsTrue(data.WasDisposed);
        }
        #endregion

        #region Helper methods

        private UnknownClipboardData GetUnknownClipboardDataAsInt(int value)
            => new UnknownClipboardData(value, new string[] { "number" });

        #endregion

        #region Helper types

        class FakeDisposableObject : IDisposable
        {
            public bool WasDisposed { get; private set; }

            public void Dispose()
            {
                WasDisposed = true;
            }
        }
        #endregion
    }
}