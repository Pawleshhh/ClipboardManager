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

            public bool WasDisposed { get; private set; } = false;

            public override void Dispose()
            {
                WasDisposed = true;
            }

        }

        #endregion

    }
}