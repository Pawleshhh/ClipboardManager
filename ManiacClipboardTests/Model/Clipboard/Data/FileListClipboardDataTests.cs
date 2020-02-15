using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManiacClipboard.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManiacClipboard.Model.Tests
{
    [TestClass()]
    public class FileListClipboardDataTests
    {
        [TestMethod]
        public void Split_SplittingCollectionData_GetsExpectedArrayOfData()
        {
            ClipboardSource source = new ClipboardSource("appName");
            DateTime dateTime = new DateTime(2020, 1, 1);
            var clipboardData = new FileListClipboardData(new KeyValuePair<string, bool>[]
            {
                new KeyValuePair<string, bool>("path1", true)
            }, dateTime, source)
            {
                KeepThat = true
            };

            var result = clipboardData.Split();

            Assert.IsTrue(((PathClipboardData)result[0]).IsDirectory);
            Assert.AreSame(source, result[0].Source);
            Assert.AreEqual(dateTime, result[0].CopyTime);
            Assert.AreEqual("path1", result[0].Data);
            Assert.IsTrue(result[0].KeepThat);
        }
    }
}