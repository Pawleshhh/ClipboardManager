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
                () => new MockClipboardData(null, ClipboardDataType.Audio));
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

        #endregion

        #region Mock

        class MockClipboardData : ClipboardData
        {

            public MockClipboardData(object data, ClipboardDataType type) : base(data, type) { }

            public MockClipboardData(object data, ClipboardDataType type, DateTime copyTime) : base(data, type, copyTime) { }

            public MockClipboardData(object data, ClipboardDataType type, ClipboardSource source) : base(data, type, source) { }

            public MockClipboardData(object data, ClipboardDataType type, DateTime copyTime, ClipboardSource source) : base(data, type, copyTime, source) { }

            public bool WasDisposed { get; private set; } = false;

            public override void Dispose()
            {
                WasDisposed = true;
            }

        }

        #endregion

    }
}