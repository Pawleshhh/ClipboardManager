using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManiacClipboard.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ManiacClipboard.Model.Tests
{

    /// <summary>
    /// Unit tests of the <see cref="CollectionClipboardData{T}"/> class.
    /// </summary>
    [TestClass()]
    public class CollectionClipboardDataTests
    {

        #region Unit tests

        [TestMethod]
        public void Equals_CheckingEqualityWhenParameterIsNull_ReturnsFalse()
        {
            var clipboardData = new MockCollectionClipboardData(new int[] { 1, 2, 3 }, ClipboardDataType.Unknown);

            bool result = clipboardData.Equals(null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_CheckingEqualityWhenCollectionsAreSame_ReturnsTrue()
        {
            var clipboardData1 = new MockCollectionClipboardData(new int[] { 1, 2, 3 }, ClipboardDataType.Unknown);
            var clipboardData2 = clipboardData1;

            bool result = clipboardData1.Equals(clipboardData2);

            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 }, true)]
        [DataRow(new int[] { 3, 2, 1 }, new int[] { 1, 2, 3 }, true)]
        [DataRow(new int[] { 4, 2, 3 }, new int[] { 1, 2, 3 }, false)]
        public void Equals_CheckingEqualityWhenCollectionsAreEqualOrEquivalent_ReturnsExpectedResult(int[] array1, int[] array2, bool expected)
        {
            var clipboardData1 = new MockCollectionClipboardData(array1, ClipboardDataType.Unknown);
            var clipboardData2 = new MockCollectionClipboardData(array2, ClipboardDataType.Unknown);

            bool result = clipboardData1.Equals(clipboardData2);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Equals_CheckingEqualityWhenTypesAreNotTheSame_ReturnsFalse()
        {
            var clipboardData = new MockCollectionClipboardData(new int[] { 1, 2, 3 }, ClipboardDataType.Unknown);
            object other = new object();

            bool result = clipboardData.Equals(other);

            Assert.IsFalse(result);
        }

        #endregion

        #region Mock

        class MockCollectionClipboardData : CollectionClipboardData<int>
        {
            #region Constructors

            public MockCollectionClipboardData(IReadOnlyCollection<int> data, ClipboardDataType type) : base(data, type)
            {
            }

            public MockCollectionClipboardData(IReadOnlyCollection<int> data, ClipboardDataType type, DateTime copyTime) : base(data, type, copyTime)
            {
            }

            public MockCollectionClipboardData(IReadOnlyCollection<int> data, ClipboardDataType type, ClipboardSource source) : base(data, type, source)
            {
            }

            public MockCollectionClipboardData(IReadOnlyCollection<int> data, ClipboardDataType type, DateTime copyTime, ClipboardSource source) : base(data, type, copyTime, source)
            {
            }

            #endregion

            #region Public methods

            public override ClipboardData[] Split() => new ClipboardData[Data.Count];

            #endregion

        }

        #endregion

    }
}