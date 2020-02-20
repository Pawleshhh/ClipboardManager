using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManiacClipboard.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManiacClipboard.Model;

namespace ManiacClipboard.ViewModel.Tests
{
    [TestClass()]
    public class ClipboardManagerViewModelTests
    {

        #region Tests

        [TestMethod]
        public void SplitData_SplitsData_CollectionContainsSplitData()
        {
            var managerVM = new ClipboardManagerViewModel(new MockClipboardService());
            FileListClipboardDataViewModel fileList = new FileListClipboardDataViewModel(
                new FileListClipboardData(new KeyValuePair<string, bool>[]
                {
                    new KeyValuePair<string, bool>("path1", true),
                    new KeyValuePair<string, bool>("path2", false),
                    new KeyValuePair<string, bool>("path3", true)
                }));
            var expected = new ClipboardDataViewModel[]
            {
                new PathClipboardDataViewModel(new PathClipboardData("path1", true)),
                new PathClipboardDataViewModel(new PathClipboardData("path2", false)),
                new PathClipboardDataViewModel(new PathClipboardData("path3", true)),
            };

            managerVM.ClipboardCollectionVM.Add(fileList);

            managerVM.SplitData(fileList);
            managerVM.ClipboardCollectionVM.TaskCollection.Task.Wait();

            CollectionAssert.AreEquivalent(expected, managerVM.ClipboardCollectionVM.TaskCollection.Result);
        }

        [TestMethod]
        public void SplitData_SplitsDataWhichAreNotStored_CollectionDoesNotContainSplitData()
        {
            var managerVM = new ClipboardManagerViewModel(new MockClipboardService());
            FileListClipboardDataViewModel fileList = new FileListClipboardDataViewModel(
                new FileListClipboardData(new KeyValuePair<string, bool>[]
                {
                    new KeyValuePair<string, bool>("path1", true),
                    new KeyValuePair<string, bool>("path2", false),
                    new KeyValuePair<string, bool>("path3", true)
                }));

            managerVM.SplitData(fileList);
            managerVM.ClipboardCollectionVM.TaskCollection.Task.Wait();

            Assert.AreEqual(0, managerVM.ClipboardCollectionVM.Count);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void IsMonitoring_SettingMonitoringToGivenValue_ServiceIsMonitoringIsExpectedValue(bool expected)
        {
            var service = new MockClipboardService();
            var managerVM = new ClipboardManagerViewModel(service);

            managerVM.IsMonitoring = expected;

            Assert.AreEqual(expected, service.IsMonitoring);
            Assert.AreEqual(expected, managerVM.IsMonitoring);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void AutoConvert_settingAutoConvertToGivenValue_ServiceAutoConvertIsExpectedValue(bool expected)
        {
            var service = new MockClipboardService();
            var managerVM = new ClipboardManagerViewModel(service);

            managerVM.AutoConvert = expected;

            Assert.AreEqual(expected, service.AutoConvert);
            Assert.AreEqual(expected, managerVM.AutoConvert);
        }

        [TestMethod]
        public async void GetCurrentData_GettingCurrentData_ReturnsCurrentData()
        {
            ClipboardData clipboardData = new TextClipboardData("text1");
            MockClipboardService service = new MockClipboardService();
            service.CurrentData = clipboardData;
            ClipboardManagerViewModel managerVM = new ClipboardManagerViewModel(service);

            managerVM.AddCurrentData();
        }

        #endregion

        #region Mock

        class MockClipboardService : IClipboardService
        {

            public ClipboardData CurrentData { get; set; }

            public bool IsMonitoring { get; private set; }

            public bool AutoConvert { get; set; }

            public event EventHandler<ClipboardChangedEventArgs> ClipboardChanged;

            public void ClearClipboard()
            {
                throw new NotImplementedException();
            }

            public Task ClearClipboardAsync()
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public ClipboardData GetClipboardData()
            {
                return CurrentData;
            }

            public Task<ClipboardData> GetClipboardDataAsync()
            {
                return Task.Run(() => CurrentData);
            }

            public WindowsClipboardDataType GetClipboardDataType()
            {
                throw new NotImplementedException();
            }

            public Task<WindowsClipboardDataType> GetClipboardDataTypeAsync()
            {
                throw new NotImplementedException();
            }

            public bool IsClipboardEmpty()
            {
                throw new NotImplementedException();
            }

            public Task<bool> IsClipboardEmptyAsync()
            {
                throw new NotImplementedException();
            }

            public void SetClipboardData(ClipboardData data)
            {
                throw new NotImplementedException();
            }

            public Task SetClipboardDataAsync(ClipboardData data)
            {
                throw new NotImplementedException();
            }

            public void StartMonitoring()
            {
                IsMonitoring = true;
            }

            public void StopMonitoring()
            {
                IsMonitoring = false;
            }
        }

        #endregion

    }
}