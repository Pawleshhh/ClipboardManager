using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManiacClipboard.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ManiacClipboard.Model;
using System.IO;
using System.Linq;

namespace ManiacClipboard.ViewModel.Tests
{
    [TestClass()]
    public class ClipboardCollectionViewModelTests
    {

        #region Tests

        [DataTestMethod]
        [DataRow(ClipboardCollectionFilters.All, ClipboardCollectionFilters.None, false)]
        [DataRow(ClipboardCollectionFilters.None, ClipboardCollectionFilters.None, true)]
        [DataRow(ClipboardCollectionFilters.All, ClipboardCollectionFilters.Text, true)]
        [DataRow(ClipboardCollectionFilters.Text, ClipboardCollectionFilters.All, false)]
        public void HasShowFilter_ContainsOrDoesNotContainGivenShowFilter_ReturnsExpectedValue(ClipboardCollectionFilters stored, ClipboardCollectionFilters check, bool expected)
        {
            var collectionVM = GetCollectionVM();
            SetShowField(collectionVM, stored);

            bool result = collectionVM.HasShowFilter(check);

            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow(ClipboardCollectionFilters.All, ClipboardCollectionFilters.None, false)]
        [DataRow(ClipboardCollectionFilters.None, ClipboardCollectionFilters.None, true)]
        [DataRow(ClipboardCollectionFilters.All, ClipboardCollectionFilters.Text, true)]
        [DataRow(ClipboardCollectionFilters.Text, ClipboardCollectionFilters.All, false)]
        public void HasStoreFilter_ContainsOrDoesNotContainGivenStoreFilter_ReturnsExpectedValue(ClipboardCollectionFilters stored, ClipboardCollectionFilters check, bool expected)
        {
            var collectionVM = GetCollectionVM();
            SetStoreField(collectionVM, stored);

            bool result = collectionVM.HasStoreFilter(check);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetProperFilterOfGivenData_GettingFilterForTextData_ReturnsTextFilter()
        {
            TextClipboardDataViewModel textVM = new TextClipboardDataViewModel(new TextClipboardData("text"));

            GetProperFilterOfGivenData_HelperMethodTest(textVM, ClipboardCollectionFilters.Text);
        }

        [TestMethod]
        public void GetProperFilterOfGivenData_GettingFilterForFilePathtData_ReturnsFilePathFilter()
        {
            PathClipboardDataViewModel pathtVM = new PathClipboardDataViewModel(new PathClipboardData("path", false));

            GetProperFilterOfGivenData_HelperMethodTest(pathtVM, ClipboardCollectionFilters.FilePath);
        }

        [TestMethod]
        public void GetProperFilterOfGivenData_GettingFilterForFileListData_ReturnsFileListFilter()
        {
            FileListClipboardDataViewModel fileListVM = new FileListClipboardDataViewModel(new FileListClipboardData(new KeyValuePair<string, bool>[] { }));

            GetProperFilterOfGivenData_HelperMethodTest(fileListVM, ClipboardCollectionFilters.FileList);
        }

        [TestMethod]
        public void GetProperFilterOfGivenData_GettingFilterForImageData_ReturnsImageFilter()
        {
            using(MemoryStream ms = new MemoryStream())
            {
                ImageClipboardDataViewModel imageVM = new ImageClipboardDataViewModel(new ImageClipboardData(ms));

                GetProperFilterOfGivenData_HelperMethodTest(imageVM, ClipboardCollectionFilters.Image);
            }
        }

        [TestMethod]
        public void GetProperFilterOfGivenData_GettingFilterForUnknownData_ReturnsUnknownFilter()
        {
            UnknownClipboardDataViewModel unknownVM = new UnknownClipboardDataViewModel(new UnknownClipboardData(10, new string[] { "number" }));

            GetProperFilterOfGivenData_HelperMethodTest(unknownVM, ClipboardCollectionFilters.Unknown);
        }

        [TestMethod]
        public void AddShowFilter_AddingNoneFilter_ShowFilterContainsOnlyNoneFilter()
        {
            var collectionVM = GetCollectionVM();
            SetShowField(collectionVM, ClipboardCollectionFilters.All);

            collectionVM.AddShowFilter(ClipboardCollectionFilters.None);

            Assert.AreEqual(ClipboardCollectionFilters.None, collectionVM.ShowTypeOfClipboardData);
        }

        [DataTestMethod]
        [DataRow(ClipboardCollectionFilters.FileList, ClipboardCollectionFilters.Text, ClipboardCollectionFilters.FileList | ClipboardCollectionFilters.Text)]
        [DataRow(ClipboardCollectionFilters.All, ClipboardCollectionFilters.Text, ClipboardCollectionFilters.All)]
        [DataRow(ClipboardCollectionFilters.None, ClipboardCollectionFilters.All, ClipboardCollectionFilters.All)]
        [DataRow(ClipboardCollectionFilters.Image, ClipboardCollectionFilters.Image, ClipboardCollectionFilters.Image)]
        public void AddShowFilter_AddingGivenFilter_ShowFilterContainsGivenFilter(ClipboardCollectionFilters beginValue, ClipboardCollectionFilters given, ClipboardCollectionFilters expected)
        {
            var collectionVM = GetCollectionVM();
            SetShowField(collectionVM, beginValue);

            collectionVM.AddShowFilter(given);

            Assert.AreEqual(expected, collectionVM.ShowTypeOfClipboardData);
        }

        [DataTestMethod]
        [DataRow(ClipboardCollectionFilters.FileList, ClipboardCollectionFilters.Text, ClipboardCollectionFilters.FileList | ClipboardCollectionFilters.Text)]
        [DataRow(ClipboardCollectionFilters.All, ClipboardCollectionFilters.Text, ClipboardCollectionFilters.All)]
        [DataRow(ClipboardCollectionFilters.None, ClipboardCollectionFilters.All, ClipboardCollectionFilters.All)]
        public void AddStoreFilter_AddingGivenFilter_StoreFilterContainsGivenFilter(ClipboardCollectionFilters beginValue, ClipboardCollectionFilters given, ClipboardCollectionFilters expected)
        {
            var collectionVM = GetCollectionVM();
            SetStoreField(collectionVM, beginValue);

            collectionVM.AddStoreFilter(given);

            Assert.AreEqual(expected, collectionVM.StoreTypeOfClipboardData);
        }

        [DataTestMethod]
        [DataRow(ClipboardCollectionFilters.All, ClipboardCollectionFilters.All, ClipboardCollectionFilters.None)]
        [DataRow(ClipboardCollectionFilters.None, ClipboardCollectionFilters.None, ClipboardCollectionFilters.All)]
        [DataRow(ClipboardCollectionFilters.All, ClipboardCollectionFilters.Text, ClipboardCollectionFilters.All ^ ClipboardCollectionFilters.Text)]
        [DataRow(ClipboardCollectionFilters.FileList, ClipboardCollectionFilters.FileList, ClipboardCollectionFilters.None)]
        [DataRow(ClipboardCollectionFilters.Image, ClipboardCollectionFilters.Unknown, ClipboardCollectionFilters.Image)]
        public void RemoveShowFilter_RemovingGivenFilter_ShowFilterContainsExpectedFilters(ClipboardCollectionFilters beginValue, ClipboardCollectionFilters given, ClipboardCollectionFilters expected)
        {
            var collectionVM = GetCollectionVM();
            SetShowField(collectionVM, beginValue);

            collectionVM.RemoveShowFilter(given);

            Assert.AreEqual(expected, collectionVM.ShowTypeOfClipboardData);
        }

        [DataTestMethod]
        [DataRow(ClipboardCollectionFilters.All, ClipboardCollectionFilters.All, ClipboardCollectionFilters.None)]
        [DataRow(ClipboardCollectionFilters.None, ClipboardCollectionFilters.None, ClipboardCollectionFilters.All)]
        [DataRow(ClipboardCollectionFilters.All, ClipboardCollectionFilters.Text, ClipboardCollectionFilters.All ^ ClipboardCollectionFilters.Text)]
        [DataRow(ClipboardCollectionFilters.FileList, ClipboardCollectionFilters.FileList, ClipboardCollectionFilters.None)]
        [DataRow(ClipboardCollectionFilters.Image, ClipboardCollectionFilters.Unknown, ClipboardCollectionFilters.Image)]
        public void RemoveStoreFilter_RemovingGivenFilter_StoreFilterContainsExpectedFilters(ClipboardCollectionFilters beginValue, ClipboardCollectionFilters given, ClipboardCollectionFilters expected)
        {
            var collectionVM = GetCollectionVM();
            SetStoreField(collectionVM, beginValue);

            collectionVM.RemoveStoreFilter(given);

            Assert.AreEqual(expected, collectionVM.StoreTypeOfClipboardData);
        }

        [TestMethod]
        public void Add_AddsGivenData_CollectionContainsGivenData()
        {
            var collectionVM = GetCollectionVM();
            ClipboardDataViewModel dataVM = new TextClipboardDataViewModel(new TextClipboardData("text"));

            bool result = collectionVM.Add(dataVM);

            Assert.IsTrue(result);
            CollectionAssert.Contains(collectionVM.TaskCollection.Result, dataVM);
        }

        [TestMethod]
        public void Add_AddsGivenDataButItsDuplicate_CollectionDoesNotContainGivenData()
        {
            var collectionVM = GetCollectionVM();
            ClipboardDataViewModel dataVM1 = new TextClipboardDataViewModel(new TextClipboardData("text"));
            ClipboardDataViewModel dataVM2 = new TextClipboardDataViewModel(new TextClipboardData("text"));
            SetMainCollection(collectionVM, new HashSet<ClipboardDataViewModel>() { dataVM1 });

            bool result = collectionVM.Add(dataVM2);

            Assert.IsFalse(result);
            CollectionAssert.DoesNotContain(collectionVM.TaskCollection.Result, dataVM2);
        }

        [TestMethod]
        public void Add_AddsGivenDataButIsNotAbleToBeStored_CollectionDoesNotContainGivenData()
        {
            var collectionVM = GetCollectionVM();
            SetStoreField(collectionVM, ClipboardCollectionFilters.Image);
            ClipboardDataViewModel dataVM = new TextClipboardDataViewModel(new TextClipboardData("text"));

            bool result = collectionVM.Add(dataVM);

            Assert.IsFalse(result);
            CollectionAssert.DoesNotContain(collectionVM.TaskCollection.Result, dataVM);
        }

        [TestMethod]
        public void Add_AddsGivenDataButIsNotAbleToBeShown_MainCollectionContainsDataButObservableDoesNot()
        {
            var collectionVM = GetCollectionVM();
            SetShowField(collectionVM, ClipboardCollectionFilters.FilePath);
            ClipboardDataViewModel dataVM = new TextClipboardDataViewModel(new TextClipboardData("text"));

            bool result = collectionVM.Add(dataVM);

            Assert.IsTrue(result);
            CollectionAssert.Contains(GetMainCollection(collectionVM).ToArray(), dataVM);
            CollectionAssert.DoesNotContain(collectionVM.TaskCollection.Result, dataVM);
        }

        [TestMethod]
        public void Remove_RemovesGivenItem_CollectionDoesNotContainGivenItemAnymore()
        {
            var collectionVM = GetCollectionVM();
            ClipboardDataViewModel dataVM = new TextClipboardDataViewModel(new TextClipboardData("text"));
            collectionVM.Add(dataVM);

            bool result = collectionVM.Remove(dataVM);

            Assert.IsTrue(result);
            CollectionAssert.DoesNotContain(collectionVM.TaskCollection.Result, dataVM);
        }

        [TestMethod]
        public void Remove_RemovesDuplicateOfGivenItem_CollectionDoesNotContainGivenItemAnymore()
        {
            var collectionVM = GetCollectionVM();
            ClipboardDataViewModel dataVM1 = new TextClipboardDataViewModel(new TextClipboardData("text"));
            ClipboardDataViewModel dataVM2 = new TextClipboardDataViewModel(new TextClipboardData("text"));
            collectionVM.Add(dataVM1);

            bool result = collectionVM.Remove(dataVM2);

            Assert.IsTrue(result);
            CollectionAssert.DoesNotContain(collectionVM.TaskCollection.Result, dataVM1);
        }

        [TestMethod]
        public void Remove_RemovesItemThatIsNotStored_ReturnsFalse()
        {
            var collectionVM = GetCollectionVM();
            ClipboardDataViewModel dataVM = new TextClipboardDataViewModel(new TextClipboardData("text"));

            bool result = collectionVM.Remove(dataVM);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AddRange_AddsItemsWhichAllCanBeStored_AllItemsAreAdded()
        {
            var collectionVM = GetCollectionVM();
            ClipboardDataViewModel[] arrayOfItems = new ClipboardDataViewModel[]
            {
                new TextClipboardDataViewModel(new TextClipboardData("text")),
                new PathClipboardDataViewModel(new PathClipboardData("path", true)),
                new UnknownClipboardDataViewModel(new UnknownClipboardData(10, new string[] {"number"}))
            };

            collectionVM.AddRange(arrayOfItems);
            collectionVM.TaskCollection.Task.Wait();

            CollectionAssert.AreEquivalent(arrayOfItems, GetMainCollection(collectionVM).ToArray());
            CollectionAssert.AreEquivalent(arrayOfItems, collectionVM.TaskCollection.Result);
        }

        [TestMethod]
        public void AddRange_AddsItemsButOnlyTextDataCanBeStored_OnlyTextDataAreStored()
        {
            var collectionVM = GetCollectionVM();
            SetStoreField(collectionVM, ClipboardCollectionFilters.Text);
            ClipboardDataViewModel[] expected = new ClipboardDataViewModel[]
            {
                new TextClipboardDataViewModel(new TextClipboardData("text1")),
                new TextClipboardDataViewModel(new TextClipboardData("text2")),
            };
            ClipboardDataViewModel[] arrayOfItems = new ClipboardDataViewModel[]
            {
                expected[0], expected[1],
                new PathClipboardDataViewModel(new PathClipboardData("path", true)),
                new UnknownClipboardDataViewModel(new UnknownClipboardData(10, new string[] {"number"}))
            };

            collectionVM.AddRange(arrayOfItems);
            collectionVM.TaskCollection.Task.Wait();

            CollectionAssert.AreEquivalent(expected, GetMainCollection(collectionVM).ToArray());
            CollectionAssert.AreEquivalent(expected, collectionVM.TaskCollection.Result);
        }

        [TestMethod]
        public void AddRange_AddsItemsButOnlyTextDataCanBeShown_MainCollectionContainsAllItemsButObservableDoesNot()
        {
            var collectionVM = GetCollectionVM();
            SetShowField(collectionVM, ClipboardCollectionFilters.Text);
            var observableCollection_Expected = new ClipboardDataViewModel[]
            {
                new TextClipboardDataViewModel(new TextClipboardData("text1")),
                new TextClipboardDataViewModel(new TextClipboardData("text2")),
            };
            var arrayOfItems = new ClipboardDataViewModel[]
            {
                observableCollection_Expected[0], observableCollection_Expected[1],
                new PathClipboardDataViewModel(new PathClipboardData("path", true)),
                new UnknownClipboardDataViewModel(new UnknownClipboardData(10, new string[] {"number"}))
            };

            collectionVM.AddRange(arrayOfItems);
            collectionVM.TaskCollection.Task.Wait();

            CollectionAssert.AreEquivalent(arrayOfItems, GetMainCollection(collectionVM).ToArray());
            CollectionAssert.AreEquivalent(observableCollection_Expected, collectionVM.TaskCollection.Result);
        }

        [TestMethod]
        public void RemoveRange_RemovesAllGivenItems_CollectionDoesNotContainGivenItems()
        {
            var collectionVM = GetCollectionVM();
            ClipboardDataViewModel[] arrayOfItems = new ClipboardDataViewModel[]
            {
                new TextClipboardDataViewModel(new TextClipboardData("text")),
                new PathClipboardDataViewModel(new PathClipboardData("path", true)),
                new UnknownClipboardDataViewModel(new UnknownClipboardData(10, new string[] {"number"}))
            };
            collectionVM.AddRange(arrayOfItems);
            collectionVM.TaskCollection.Task.Wait();

            collectionVM.RemoveRange(arrayOfItems);
            collectionVM.TaskCollection.Task.Wait();

            CollectionAssert.AreNotEquivalent(arrayOfItems, collectionVM.TaskCollection.Result);
        }

        [TestMethod]
        public void RemoveRange_RemovesAllGivenDuplicates_CollectionDoesNotContainGivenItems()
        {
            var collectionVM = GetCollectionVM();
            ClipboardDataViewModel[] arrayOfItems = new ClipboardDataViewModel[]
            {
                new TextClipboardDataViewModel(new TextClipboardData("text")),
                new PathClipboardDataViewModel(new PathClipboardData("path", true)),
                new UnknownClipboardDataViewModel(new UnknownClipboardData(10, new string[] {"number"}))
            };
            ClipboardDataViewModel[] duplicates = new ClipboardDataViewModel[]
            {
                new TextClipboardDataViewModel(new TextClipboardData("text")),
                new PathClipboardDataViewModel(new PathClipboardData("path", true)),
                new UnknownClipboardDataViewModel(new UnknownClipboardData(10, new string[] {"number"}))
            };
            collectionVM.AddRange(arrayOfItems);
            collectionVM.TaskCollection.Task.Wait();

            collectionVM.RemoveRange(duplicates);
            collectionVM.TaskCollection.Task.Wait();

            CollectionAssert.AreNotEquivalent(arrayOfItems, collectionVM.TaskCollection.Result);
        }

        [TestMethod]
        public void Clear_ClearsCollection_CollectionIsEmpty()
        {
            var collectionVM = GetCollectionVM();
            ClipboardDataViewModel[] arrayOfItems = new ClipboardDataViewModel[]
            {
                new TextClipboardDataViewModel(new TextClipboardData("text")),
                new PathClipboardDataViewModel(new PathClipboardData("path", true)),
                new UnknownClipboardDataViewModel(new UnknownClipboardData(10, new string[] {"number"}))
            };
            collectionVM.AddRange(arrayOfItems);
            collectionVM.TaskCollection.Task.Wait();

            collectionVM.Clear();

            CollectionAssert.AreNotEquivalent(arrayOfItems, GetMainCollection(collectionVM).ToArray());
            CollectionAssert.AreNotEquivalent(arrayOfItems, collectionVM.TaskCollection.Result);
        }

        [TestMethod]
        public void AddShowFilter_AddsNoneFilter_ObservableCollectionIsEmpty()
        {
            var collectionVM = GetCollectionVM();
            ClipboardDataViewModel[] arrayOfItems = new ClipboardDataViewModel[]
            {
                new TextClipboardDataViewModel(new TextClipboardData("text")),
                new TextClipboardDataViewModel(new TextClipboardData("text2")),
                new PathClipboardDataViewModel(new PathClipboardData("path", true)),
                new UnknownClipboardDataViewModel(new UnknownClipboardData(10, new string[] {"number"}))
            };
            collectionVM.AddRange(arrayOfItems);
            collectionVM.TaskCollection.Task.Wait();

            collectionVM.AddShowFilter(ClipboardCollectionFilters.None);

            Assert.AreEqual(0, collectionVM.TaskCollection.Result.Count);
        }

        [TestMethod]
        public void AddShowFilter_AddsTextFilter_ObservableCollectionHasOnlyTextData()
        {
            var collectionVM = GetCollectionVM();
            var expectedItems = new ClipboardDataViewModel[]
            {
                new TextClipboardDataViewModel(new TextClipboardData("text")),
                new TextClipboardDataViewModel(new TextClipboardData("text2")),
            };
            var arrayOfItems = new ClipboardDataViewModel[]
            {
                expectedItems[0], expectedItems[1],
                new PathClipboardDataViewModel(new PathClipboardData("path", true)),
                new UnknownClipboardDataViewModel(new UnknownClipboardData(10, new string[] {"number"}))
            };
            collectionVM.AddRange(arrayOfItems);
            collectionVM.TaskCollection.Task.Wait();
            collectionVM.RemoveShowFilter(ClipboardCollectionFilters.All);

            collectionVM.AddShowFilter(ClipboardCollectionFilters.Text);
            collectionVM.TaskCollection.Task.Wait();

            CollectionAssert.AreEquivalent(expectedItems, collectionVM.TaskCollection.Result);
        }

        [TestMethod]
        public void RemoveShowFilter_RemovesAllFilter_ObservableCollectionIsEmpty()
        {
            var collectionVM = GetCollectionVM();
            ClipboardDataViewModel[] arrayOfItems = new ClipboardDataViewModel[]
            {
                new TextClipboardDataViewModel(new TextClipboardData("text")),
                new TextClipboardDataViewModel(new TextClipboardData("text2")),
                new PathClipboardDataViewModel(new PathClipboardData("path", true)),
                new UnknownClipboardDataViewModel(new UnknownClipboardData(10, new string[] {"number"}))
            };
            collectionVM.AddRange(arrayOfItems);
            collectionVM.TaskCollection.Task.Wait();

            collectionVM.RemoveShowFilter(ClipboardCollectionFilters.All);

            Assert.AreEqual(0, collectionVM.TaskCollection.Result.Count);
        }

        [TestMethod]
        public void RemoveShowFilter_RemovesTextFilter_ObservableCollectionDoesNotHaveTextData()
        {
            var collectionVM = GetCollectionVM();
            var expectedItems = new ClipboardDataViewModel[]
            {
                new PathClipboardDataViewModel(new PathClipboardData("path", true)),
                new UnknownClipboardDataViewModel(new UnknownClipboardData(10, new string[] {"number"}))
            };
            var arrayOfItems = new ClipboardDataViewModel[]
            {
                expectedItems[0], expectedItems[1],
                new TextClipboardDataViewModel(new TextClipboardData("text")),
                new TextClipboardDataViewModel(new TextClipboardData("text2")),
            };
            collectionVM.AddRange(arrayOfItems);
            collectionVM.TaskCollection.Task.Wait();

            collectionVM.RemoveShowFilter(ClipboardCollectionFilters.Text);
            collectionVM.TaskCollection.Task.Wait();

            CollectionAssert.AreEquivalent(expectedItems, collectionVM.TaskCollection.Result);
        }

        [TestMethod]
        public void AddStoreFilter_AddsNoneFilter_BothCollectionsAreEmpty()
        {
            var collectionVM = GetCollectionVM();
            ClipboardDataViewModel[] arrayOfItems = new ClipboardDataViewModel[]
            {
                new TextClipboardDataViewModel(new TextClipboardData("text")),
                new TextClipboardDataViewModel(new TextClipboardData("text2")),
                new PathClipboardDataViewModel(new PathClipboardData("path", true)),
                new UnknownClipboardDataViewModel(new UnknownClipboardData(10, new string[] {"number"}))
            };
            collectionVM.AddRange(arrayOfItems);
            collectionVM.TaskCollection.Task.Wait();

            collectionVM.AddStoreFilter(ClipboardCollectionFilters.None);

            Assert.AreEqual(0, collectionVM.Count);
            Assert.AreEqual(0, collectionVM.TaskCollection.Result.Count);
        }

        [TestMethod]
        public void RemoveStoreFilter_RemovesAllFilter_BothCollectionsAreEmpty()
        {
            var collectionVM = GetCollectionVM();
            ClipboardDataViewModel[] arrayOfItems = new ClipboardDataViewModel[]
            {
                new TextClipboardDataViewModel(new TextClipboardData("text")),
                new TextClipboardDataViewModel(new TextClipboardData("text2")),
                new PathClipboardDataViewModel(new PathClipboardData("path", true)),
                new UnknownClipboardDataViewModel(new UnknownClipboardData(10, new string[] {"number"}))
            };
            collectionVM.AddRange(arrayOfItems);
            collectionVM.TaskCollection.Task.Wait();

            collectionVM.RemoveStoreFilter(ClipboardCollectionFilters.All);

            Assert.AreEqual(0, collectionVM.Count);
            Assert.AreEqual(0, collectionVM.TaskCollection.Result.Count);
        }

        [TestMethod]
        public void RemoveStoreFilter_RemovesTextFilter_BothCollectionsDoNotHaveTextData()
        {
            var collectionVM = GetCollectionVM();
            var expectedItems = new ClipboardDataViewModel[]
            {
                new PathClipboardDataViewModel(new PathClipboardData("path", true)),
                new UnknownClipboardDataViewModel(new UnknownClipboardData(10, new string[] {"number"}))
            };
            var arrayOfItems = new ClipboardDataViewModel[]
            {
                expectedItems[0], expectedItems[1],
                new TextClipboardDataViewModel(new TextClipboardData("text")),
                new TextClipboardDataViewModel(new TextClipboardData("text2")),
            };
            collectionVM.AddRange(arrayOfItems);
            collectionVM.TaskCollection.Task.Wait();

            collectionVM.RemoveStoreFilter(ClipboardCollectionFilters.Text);
            collectionVM.TaskCollection.Task.Wait();

            CollectionAssert.AreEquivalent(expectedItems, GetMainCollection(collectionVM).ToArray());
            CollectionAssert.AreEquivalent(expectedItems, collectionVM.TaskCollection.Result);
        }

        [TestMethod]
        public void Add_AddingDataInDescendingOrder_DataIsOrderedAsExpected()
        {
            var collectionVM = GetCollectionVM();
            collectionVM.IsAscendingOrder = false;
            ClipboardDataViewModel item1 = new TextClipboardDataViewModel(new TextClipboardData("data1", new DateTime(2020, 1, 2)));
            ClipboardDataViewModel item2 = new TextClipboardDataViewModel(new TextClipboardData("data2", new DateTime(2020, 1, 1)));
            ClipboardDataViewModel item3 = new TextClipboardDataViewModel(new TextClipboardData("data3", new DateTime(2020, 1, 5)));
            ClipboardDataViewModel item4 = new TextClipboardDataViewModel(new TextClipboardData("data4", new DateTime(2020, 1, 4)));
            var expectedCollection = new ClipboardDataViewModel[] { item3, item4, item1, item2 };

            collectionVM.Add(item1);
            collectionVM.Add(item2);
            collectionVM.Add(item3);
            collectionVM.Add(item4);

            CollectionAssert.AreEqual(expectedCollection, collectionVM.TaskCollection.Result);
        }

        [TestMethod]
        public void Add_AddingDataInAscendingOrder_DataIsOrderedAsExpected()
        {
            var collectionVM = GetCollectionVM();
            collectionVM.IsAscendingOrder = true;
            ClipboardDataViewModel item1 = new TextClipboardDataViewModel(new TextClipboardData("data1", new DateTime(2020, 1, 2)));
            ClipboardDataViewModel item2 = new TextClipboardDataViewModel(new TextClipboardData("data2", new DateTime(2020, 1, 1)));
            ClipboardDataViewModel item3 = new TextClipboardDataViewModel(new TextClipboardData("data3", new DateTime(2020, 1, 5)));
            ClipboardDataViewModel item4 = new TextClipboardDataViewModel(new TextClipboardData("data4", new DateTime(2020, 1, 4)));
            var expectedCollection = new ClipboardDataViewModel[] { item2, item1, item4, item3 };

            collectionVM.Add(item1);
            collectionVM.Add(item2);
            collectionVM.Add(item3);
            collectionVM.Add(item4);

            CollectionAssert.AreEqual(expectedCollection, collectionVM.TaskCollection.Result);
        }

        [TestMethod]
        public void AddRange_AddingDataInDescendingOrder_DataIsOrderedAsExpected()
        {
            var collectionVM = GetCollectionVM();
            collectionVM.IsAscendingOrder = false;
            ClipboardDataViewModel item1 = new TextClipboardDataViewModel(new TextClipboardData("data1", new DateTime(2020, 1, 2)));
            ClipboardDataViewModel item2 = new TextClipboardDataViewModel(new TextClipboardData("data2", new DateTime(2020, 1, 1)));
            ClipboardDataViewModel item3 = new TextClipboardDataViewModel(new TextClipboardData("data3", new DateTime(2020, 1, 5)));
            ClipboardDataViewModel item4 = new TextClipboardDataViewModel(new TextClipboardData("data4", new DateTime(2020, 1, 4)));
            var rangeToAdd1 = new ClipboardDataViewModel[] { item2, item3 };
            var rangeToAdd2 = new ClipboardDataViewModel[] { item1, item4 };
            var expectedCollection = new ClipboardDataViewModel[] { item3, item4, item1, item2 };

            collectionVM.AddRange(rangeToAdd1);
            collectionVM.AddRange(rangeToAdd2);
            collectionVM.TaskCollection.Task.Wait();

            CollectionAssert.AreEqual(expectedCollection, collectionVM.TaskCollection.Result);
        }

        [TestMethod]
        public void AddRange_AddingDataInAscendingOrder_DataIsOrderedAsExpected()
        {
            var collectionVM = GetCollectionVM();
            collectionVM.IsAscendingOrder = true;
            ClipboardDataViewModel item1 = new TextClipboardDataViewModel(new TextClipboardData("data1", new DateTime(2020, 1, 2)));
            ClipboardDataViewModel item2 = new TextClipboardDataViewModel(new TextClipboardData("data2", new DateTime(2020, 1, 1)));
            ClipboardDataViewModel item3 = new TextClipboardDataViewModel(new TextClipboardData("data3", new DateTime(2020, 1, 5)));
            ClipboardDataViewModel item4 = new TextClipboardDataViewModel(new TextClipboardData("data4", new DateTime(2020, 1, 4)));
            var rangeToAdd1 = new ClipboardDataViewModel[] { item2, item3 };
            var rangeToAdd2 = new ClipboardDataViewModel[] { item1, item4 };
            var expectedCollection = new ClipboardDataViewModel[] { item2, item1, item4, item3 };

            collectionVM.AddRange(rangeToAdd1);
            collectionVM.AddRange(rangeToAdd2);
            collectionVM.TaskCollection.Task.Wait();

            CollectionAssert.AreEqual(expectedCollection, collectionVM.TaskCollection.Result);
        }

        [TestMethod]
        public void IsAscendingOrder_SettingAsTrue_CollectionIsSortedInAscendingOrder()
        {
            var collectionVM = GetCollectionVM();
            collectionVM.IsAscendingOrder = false;
            ClipboardDataViewModel item1 = new TextClipboardDataViewModel(new TextClipboardData("data1", new DateTime(2020, 1, 2)));
            ClipboardDataViewModel item2 = new TextClipboardDataViewModel(new TextClipboardData("data2", new DateTime(2020, 1, 1)));
            ClipboardDataViewModel item3 = new TextClipboardDataViewModel(new TextClipboardData("data3", new DateTime(2020, 1, 5)));
            ClipboardDataViewModel item4 = new TextClipboardDataViewModel(new TextClipboardData("data4", new DateTime(2020, 1, 4)));
            var expectedCollection = new ClipboardDataViewModel[] { item2, item1, item4, item3 };
            collectionVM.AddRange(new ClipboardDataViewModel[] { item1, item2, item3, item4 });
            collectionVM.TaskCollection.Task.Wait();

            collectionVM.IsAscendingOrder = true;
            collectionVM.TaskCollection.Task.Wait();

            CollectionAssert.AreEqual(expectedCollection, collectionVM.TaskCollection.Result);
        }

        [TestMethod]
        public void IsAscendingOrder_SettingAsFalse_CollectionIsSortedInDescendingOrder()
        {
            var collectionVM = GetCollectionVM();
            collectionVM.IsAscendingOrder = true;
            ClipboardDataViewModel item1 = new TextClipboardDataViewModel(new TextClipboardData("data1", new DateTime(2020, 1, 2)));
            ClipboardDataViewModel item2 = new TextClipboardDataViewModel(new TextClipboardData("data2", new DateTime(2020, 1, 1)));
            ClipboardDataViewModel item3 = new TextClipboardDataViewModel(new TextClipboardData("data3", new DateTime(2020, 1, 5)));
            ClipboardDataViewModel item4 = new TextClipboardDataViewModel(new TextClipboardData("data4", new DateTime(2020, 1, 4)));
            var expectedCollection = new ClipboardDataViewModel[] { item3, item4, item1, item2 };

            collectionVM.AddRange(new ClipboardDataViewModel[] { item1, item2, item3, item4 });
            collectionVM.TaskCollection.Task.Wait();

            collectionVM.IsAscendingOrder = false;
            collectionVM.TaskCollection.Task.Wait();

            CollectionAssert.AreEqual(expectedCollection, collectionVM.TaskCollection.Result);
        }

        #endregion

        #region FieldInfos

        private FieldInfo _storeField =
            typeof(ClipboardCollectionViewModel).GetField("_storeTypeOfClipboardData",
                BindingFlags.NonPublic | BindingFlags.Instance);

        private FieldInfo _showField =
            typeof(ClipboardCollectionViewModel).GetField("_showTypeOfClipboardData",
                BindingFlags.NonPublic | BindingFlags.Instance);

        private FieldInfo _mainCollection =
            typeof(ClipboardCollectionViewModel).GetField("_mainCollection",
                BindingFlags.NonPublic | BindingFlags.Instance);

        private void SetStoreField(ClipboardCollectionViewModel obj, ClipboardCollectionFilters filter)
            => _storeField.SetValue(obj, filter);

        private void SetShowField(ClipboardCollectionViewModel obj, ClipboardCollectionFilters filter)
            => _showField.SetValue(obj, filter);

        private void SetMainCollection(ClipboardCollectionViewModel obj, HashSet<ClipboardDataViewModel> collection)
            => _mainCollection.SetValue(obj, collection);

        private HashSet<ClipboardDataViewModel> GetMainCollection(ClipboardCollectionViewModel obj)
            => (HashSet<ClipboardDataViewModel>)_mainCollection.GetValue(obj);

        #endregion

        #region Helper methods

        private ClipboardCollectionViewModel GetCollectionVM()
        {
            var collectionVM = new ClipboardCollectionViewModel();
            SetStoreField(collectionVM, ClipboardCollectionFilters.All);
            SetShowField(collectionVM, ClipboardCollectionFilters.All);

            return collectionVM;
        }

        private void GetProperFilterOfGivenData_HelperMethodTest(ClipboardDataViewModel data, ClipboardCollectionFilters expected)
        {
            var collectionVM = GetCollectionVM();

            ClipboardCollectionFilters result = collectionVM.GetProperFilterOfGivenData(data);

            Assert.AreEqual(expected, result);
        }


        #endregion

    }
}