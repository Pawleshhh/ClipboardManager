using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManiacClipboard.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ManiacClipboard.Model.Tests
{
    [TestClass()]
    public class ImageClipboardDataTests
    {

        [TestMethod]
        public void Eqauls_CheckingEqualityWhenParameterIsNull_ReturnsFalse()
        {
            using (MemoryStream ms1 = new MemoryStream(new byte[] { 1, 1, 1 }))
            {
                var clipboardData1 = new ImageClipboardData(ms1);

                bool result = clipboardData1.Equals(null);

                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void Eqauls_CheckingEqualityWhenImagesAreEqual_ReturnsTrue()
        {
            using (MemoryStream ms1 = new MemoryStream(new byte[] { 1, 1, 1 }))
            using (MemoryStream ms2 = new MemoryStream(new byte[] { 1, 1, 1 }))
            {
                var clipboardData1 = new ImageClipboardData(ms1);
                var clipboardData2 = new ImageClipboardData(ms2);

                bool result = clipboardData1.Equals(clipboardData2);

                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void Eqauls_CheckingEqualityWhenImagesAreNotEqual_ReturnsFalse()
        {
            using (MemoryStream ms1 = new MemoryStream(new byte[] { 1, 1, 1 }))
            using (MemoryStream ms2 = new MemoryStream(new byte[] { 3, 1, 1 }))
            {
                var clipboardData1 = new ImageClipboardData(ms1);
                var clipboardData2 = new ImageClipboardData(ms2);

                bool result = clipboardData1.Equals(clipboardData2);

                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void OverriddenEquals_CheckingEqualityWhenImagesAreEqual_ReturnsTrue()
        {
            using (MemoryStream ms1 = new MemoryStream(new byte[] { 1, 1, 1 }))
            using (MemoryStream ms2 = new MemoryStream(new byte[] { 1, 1, 1 }))
            {
                var clipboardData1 = new ImageClipboardData(ms1);
                object clipboardData2 = new ImageClipboardData(ms2);

                bool result = clipboardData1.Equals(clipboardData2);

                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void OverriddenEquals_CheckingEqualityWhenImagesAreNotEqual_ReturnsFalse()
        {
            using (MemoryStream ms1 = new MemoryStream(new byte[] { 1, 1, 1 }))
            using (MemoryStream ms2 = new MemoryStream(new byte[] { 3, 1, 1 }))
            {
                var clipboardData1 = new ImageClipboardData(ms1);
                object clipboardData2 = new ImageClipboardData(ms2);

                bool result = clipboardData1.Equals(clipboardData2);

                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void OverriddenEquals_CheckingEqualityWhenParameterIsNull_ReturnsFalse()
        {
            using(MemoryStream ms = new MemoryStream(new byte[] { 1, 1, 1 }))
            {
                var clipboardData = new ImageClipboardData(ms);
                object obj = null;

                bool result = clipboardData.Equals(obj);

                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void OverriddenEquals_CheckingEqualityWhenObjectsAreTheSame_ReturnsTrue()
        {
            using(MemoryStream ms = new MemoryStream())
            {
                var clipboardData1 = new ImageClipboardData(ms);
                object clipboardData2 = clipboardData1;

                bool result = clipboardData1.Equals(clipboardData2);

                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void OverriddenEquals_CheckingEqualityWhenObjectsAreNotTheSameType_ReturnsFalse()
        {
            using(MemoryStream ms = new MemoryStream())
            {
                var clipboardData = new ImageClipboardData(ms);
                object obj = new object();

                bool result = clipboardData.Equals(obj);

                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void OverriddenEquals_CheckingEqualityhWhenObjectsAreEqual_ReturnsTrue()
        {
            using (MemoryStream ms1 = new MemoryStream(new byte[] { 1, 1, 1 }))
            using (MemoryStream ms2 = new MemoryStream(new byte[] { 1, 1, 1 }))
            {
                var clipboardData1 = new ImageClipboardData(ms1);
                object clipboardData2 = new ImageClipboardData(ms2);

                bool result = clipboardData1.Equals(clipboardData2);

                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void OverriddenEquals_CheckingEqualityhWhenObjectsAreNotEqual_ReturnsFalse()
        {
            using (MemoryStream ms1 = new MemoryStream(new byte[] { 1, 1, 1 }))
            using (MemoryStream ms2 = new MemoryStream(new byte[] { 3, 1, 1 }))
            {
                var clipboardData1 = new ImageClipboardData(ms1);
                object clipboardData2 = new ImageClipboardData(ms2);

                bool result = clipboardData1.Equals(clipboardData2);

                Assert.IsFalse(result);
            }
        }

    }
}