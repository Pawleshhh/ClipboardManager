using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManiacClipboard.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManiacClipboard.Model.Tests
{

    /// <summary>
    /// Unit tests of the <see cref="Model.ClipboardData"/> class.
    /// </summary>
    [TestClass()]
    public class ClipboardDataTests
    {

        #region Tests

        [TestMethod]
        public void Constructor_GivenDataIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new MockClipboardData(null, ClipboardDataType.Unknown));
        }

        [TestMethod]
        public void Constructor_GivenTypeIsNotDefined_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(
                () => new MockClipboardData(new object(), (ClipboardDataType)int.MaxValue));
        }

        [TestMethod]
        public void Dispose_InvokingDisposeMethod_DataIsDisposed()
        {
            var clipboardData = new MockClipboardData(new object(), ClipboardDataType.Unknown);

            clipboardData.Dispose();

            Assert.IsTrue(clipboardData.WasDisposed);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Equals_EqualsReturnsExpectedValue(bool expected)
        {
            var clipboardData = new MockClipboardData("data", ClipboardDataType.Text) { EqualsReturnValue = expected };

            bool result = clipboardData.Equals(new object());

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetHashCode_GetHashCodeReturnsExpectedValue()
        {
            var clipboardData = new MockClipboardData("data", ClipboardDataType.Text)
                { GetHashCodeReturnValue = 13 };

            int result = clipboardData.GetHashCode();

            Assert.AreEqual(13, result);
        }

        #endregion

        #region Mock

        class MockClipboardData : ClipboardData
        {

            public MockClipboardData(object data, ClipboardDataType type) : base(data, type) { }

            public MockClipboardData(object data, ClipboardDataType type, DateTime copyTime) : base(data, type, copyTime) { }

            public MockClipboardData(object data, ClipboardDataType type, ClipboardSource source) : base(data, type, source) { }

            public MockClipboardData(object data, ClipboardDataType type, DateTime copyTime, ClipboardSource source) : base(data, type, copyTime, source) { }

            public bool WasDisposed { get; private set; }

            public bool EqualsReturnValue { get; set; }

            public int GetHashCodeReturnValue { get; set; } = -1;

            public override void Dispose()
            {
                WasDisposed = true;
            }

            public override bool Equals(object obj) => EqualsReturnValue;

            public override int GetHashCode() => GetHashCodeReturnValue;
        }

        #endregion

    }
}