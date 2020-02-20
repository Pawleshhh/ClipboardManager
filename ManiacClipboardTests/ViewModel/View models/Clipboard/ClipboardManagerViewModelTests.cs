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
        public async Task GetCurrentData_GettingCurrentData_CollectionContainsCurrentData()
        {
            TextClipboardData clipboardData = new TextClipboardData("text1");
            TextClipboardDataViewModel clipboardDataVM = new TextClipboardDataViewModel(clipboardData);
            MockClipboardService service = new MockClipboardService();
            service.CurrentData = clipboardData;
            ClipboardManagerViewModel managerVM = new ClipboardManagerViewModel(service);

            managerVM.AddCurrentData();
            await managerVM.ClipboardTask.Task;

            Assert.IsTrue(managerVM.ClipboardCollectionVM.Contains(clipboardDataVM));
        }

        [TestMethod]
        public async Task GetCurrentData_GettingCurrentDataWhenServiceHasNoData_CollectionDoesNotContainData()
        {
            MockClipboardService service = new MockClipboardService();
            service.CurrentData = null;
            ClipboardManagerViewModel managerVM = new ClipboardManagerViewModel(service);

            managerVM.AddCurrentData();
            await managerVM.ClipboardTask.Task;

            Assert.AreEqual(0, managerVM.ClipboardCollectionVM.Count);
        }

        [TestMethod]
        public async Task SetData_SetsGivenDataOnClipboard_ClipboardStoresGivenData()
        {
            TextClipboardData clipboardData = new TextClipboardData("text1");
            TextClipboardDataViewModel clipboardDataVM = new TextClipboardDataViewModel(clipboardData);
            MockClipboardService service = new MockClipboardService() { CurrentData = null };
            ClipboardManagerViewModel managerVM = new ClipboardManagerViewModel(service);

            managerVM.SetData(clipboardDataVM);
            await managerVM.ClipboardTask.Task;

            Assert.AreSame(clipboardData, service.CurrentData);
        }

        [TestMethod]
        public async Task ClearClipboard_ClearingClipboard_ClipboardStoresNoData()
        {
            TextClipboardData clipboardData = new TextClipboardData("text1");
            MockClipboardService service = new MockClipboardService() { CurrentData = clipboardData };
            ClipboardManagerViewModel managerVM = new ClipboardManagerViewModel(service);

            managerVM.ClearClipboard();
            await managerVM.ClipboardTask.Task;

            Assert.IsNull(service.CurrentData);
        }

        [TestMethod]
        public async Task IsClipboardEmpty_CheckingIfClipboardIsEmptyWhenItIsEmpty_ReturnsTrue()
        {
            MockClipboardService service = new MockClipboardService() { CurrentData = null };
            ClipboardManagerViewModel managerVM = new ClipboardManagerViewModel(service);

            bool result = managerVM.IsClipboardEmpty;
            await managerVM.ClipboardTask.Task;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsClipboardEmpty_CheckingIfClipboardIsEmptyWhenItIsNotEmpty_ReturnsFalse()
        {
            MockClipboardService service = new MockClipboardService() { CurrentData = new TextClipboardData("text") };
            ClipboardManagerViewModel managerVM = new ClipboardManagerViewModel(service);

            bool result = managerVM.IsClipboardEmpty;
            await managerVM.ClipboardTask.Task;

            Assert.IsFalse(result);
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
                CurrentData = null;
            }

            public Task ClearClipboardAsync()
            {
                return Task.Run(() => ClearClipboard());
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

            public ClipboardDataType GetClipboardDataType()
            {
                throw new NotImplementedException();
            }

            public Task<ClipboardDataType> GetClipboardDataTypeAsync()
            {
                throw new NotImplementedException();
            }

            public bool IsClipboardEmpty()
            {
                return CurrentData == null;
            }

            public Task<bool> IsClipboardEmptyAsync()
            {
                return Task.Run(() => IsClipboardEmpty());
            }

            public void SetClipboardData(ClipboardData data)
            {
                CurrentData = data;
            }

            public Task SetClipboardDataAsync(ClipboardData data)
            {
                return Task.Run(() => SetClipboardData(data));
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