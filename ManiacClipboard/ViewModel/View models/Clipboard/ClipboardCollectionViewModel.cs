
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ManiacClipboard.ViewModel
{
    public class ClipboardCollectionViewModel : NotifyPropertyChanges
    {
        #region Constructors
        public ClipboardCollectionViewModel()
        {
            _mainCollection = new HashSet<ClipboardDataViewModel>();
            _observableCollection = new ObservableCollection<ClipboardDataViewModel>();

            TaskCollection = new NotifyTaskCompletion<ReadOnlyObservableCollection<ClipboardDataViewModel>>(
                Task.FromResult(new ReadOnlyObservableCollection<ClipboardDataViewModel>(_observableCollection)));
        }
        #endregion

        #region Private fields

        private ClipboardCollectionFilters _showTypeOfClipboardData = ClipboardCollectionFilters.All;

        private ClipboardCollectionFilters _storeTypeOfClipboardData = ClipboardCollectionFilters.All;

        private bool _isCollectionBusy;

        private readonly HashSet<ClipboardDataViewModel> _mainCollection;

        private readonly ObservableCollection<ClipboardDataViewModel> _observableCollection;

        #endregion

        #region Properties

        public NotifyTaskCompletion<ReadOnlyObservableCollection<ClipboardDataViewModel>> TaskCollection
        {
            get; private set;
        }

        public bool IsCollectionBusy
        {
            get => _isCollectionBusy;
            private set
            {
                SetProperty(() => _isCollectionBusy == value,
                    () => _isCollectionBusy = value, nameof(IsCollectionBusy));
            }
        }

        public int Count => _mainCollection.Count;

        /// <summary>
        /// Gets or sets flags that indicate what type of clipboard data is shown.
        /// </summary>
        public ClipboardCollectionFilters ShowTypeOfClipboardData
        {
            get => _showTypeOfClipboardData;
            private set
            {
                SetProperty(() => _showTypeOfClipboardData == value, () => _showTypeOfClipboardData = value);
            }
        }

        /// <summary>
        /// Gets or sets flags that indicate what type of clipboard data is stored.
        /// </summary>
        public ClipboardCollectionFilters StoreTypeOfClipboardData
        {
            get => _storeTypeOfClipboardData;
            private set
            {
                SetProperty(() => _storeTypeOfClipboardData == value, () => _storeTypeOfClipboardData = value);
            }
        }

        #endregion

        #region Public methods

        public bool Add(ClipboardDataViewModel data)
        {
            WaitForTaskCollection();

            if(IsAbleToBeStored(data) && _mainCollection.Add(data))
            {
                if(IsAbleToBeShown(data))
                    _observableCollection.Add(data);

                return true;
            }

            return false;
        }

        public bool Remove(ClipboardDataViewModel data)
        {
            WaitForTaskCollection();

            if(_mainCollection.Remove(data))
            {
                _observableCollection.Remove(data);

                return true;
            }

            return false;
        }

        public void AddRange(IEnumerable<ClipboardDataViewModel> collection)
        {
            WorkOnCollection(() =>
            {
                foreach(var item in collection)
                {
                    if(IsAbleToBeStored(item) && _mainCollection.Add(item))
                    {
                        if (IsAbleToBeShown(item))
                            _observableCollection.Add(item);
                    }
                }

                return new ReadOnlyObservableCollection<ClipboardDataViewModel>(_observableCollection);
            });
        }

        public void RemoveRange(IEnumerable<ClipboardDataViewModel> collection)
        {
            WorkOnCollection(() =>
            {
                foreach (var item in collection)
                {
                    if (_mainCollection.Remove(item))
                        _observableCollection.Remove(item);
                }

                return new ReadOnlyObservableCollection<ClipboardDataViewModel>(_observableCollection);
            });
        }

        public void Clear()
        {
            WaitForTaskCollection();

            _mainCollection.Clear();
            _observableCollection.Clear();
        }

        public void AddShowFilter(ClipboardCollectionFilters filter)
        {
            if (filter == ClipboardCollectionFilters.None)
            {
                ShowTypeOfClipboardData &= ClipboardCollectionFilters.None;

                WaitForTaskCollection();
                _observableCollection.Clear();
            }
            else
            {
                ShowTypeOfClipboardData |= filter;
                UpdateCollectionByShowFilter();
            }
        }

        public void RemoveShowFilter(ClipboardCollectionFilters filter)
        {
            if (filter == ClipboardCollectionFilters.All)
            {
                ShowTypeOfClipboardData &= ClipboardCollectionFilters.None;

                WaitForTaskCollection();
                _observableCollection.Clear();
            }
            else
            {
                if (filter == ClipboardCollectionFilters.None && HasShowFilter(ClipboardCollectionFilters.None))
                    ShowTypeOfClipboardData |= ClipboardCollectionFilters.All;
                else if (HasShowFilter(filter))
                    ShowTypeOfClipboardData ^= filter;

                UpdateCollectionByShowFilter();
            }
        }

        public void AddStoreFilter(ClipboardCollectionFilters filter)
        {
            if (filter == ClipboardCollectionFilters.None)
            {
                StoreTypeOfClipboardData &= ClipboardCollectionFilters.None;
                Clear();
            }
            else
                StoreTypeOfClipboardData |= filter;
        }

        public void RemoveStoreFilter(ClipboardCollectionFilters filter)
        {
            if (filter == ClipboardCollectionFilters.All)
            {
                StoreTypeOfClipboardData &= ClipboardCollectionFilters.None;
                Clear();
            }
            else
            {
                if (filter == ClipboardCollectionFilters.None && HasStoreFilter(ClipboardCollectionFilters.None))
                    StoreTypeOfClipboardData |= ClipboardCollectionFilters.All;
                else if (HasStoreFilter(filter))
                    StoreTypeOfClipboardData ^= filter;

                UpdateCollectionByStoreFilter();
            }
        }

        public bool HasShowFilter(ClipboardCollectionFilters filter)
        {
            if (filter == ClipboardCollectionFilters.None)
                return _showTypeOfClipboardData == 0;

            return (_showTypeOfClipboardData & filter) == filter;
        }

        public bool HasStoreFilter(ClipboardCollectionFilters filter)
        {
            if (filter == ClipboardCollectionFilters.None)
                return _storeTypeOfClipboardData == 0;

            return (_storeTypeOfClipboardData & filter) == filter;
        }

        public bool IsAbleToBeStored(ClipboardDataViewModel clipboardData)
            => HasStoreFilter(GetProperFilterOfGivenData(clipboardData));

        public bool IsAbleToBeShown(ClipboardDataViewModel clipboardData)
            => HasShowFilter(GetProperFilterOfGivenData(clipboardData));

        public ClipboardCollectionFilters GetProperFilterOfGivenData(ClipboardDataViewModel clipboardData)
        {
            switch(clipboardData.DataType)
            {
                case Model.ClipboardDataType.Text:
                    return ClipboardCollectionFilters.Text;

                case Model.ClipboardDataType.FilePath:
                    return ClipboardCollectionFilters.FilePath;

                case Model.ClipboardDataType.FileList:
                    return ClipboardCollectionFilters.FileList;

                case Model.ClipboardDataType.Image:
                    return ClipboardCollectionFilters.Image;

                default:
                    return ClipboardCollectionFilters.Unknown;
            }
        }

        #endregion

        #region Private methods

        private void UpdateCollectionByShowFilter()
        {
            WorkOnCollection(() =>
            {
                _observableCollection.Clear();
                foreach(var item in _mainCollection)
                {
                    if (!IsAbleToBeShown(item))
                    {
                        _observableCollection.Remove(item);
                    }
                    else
                    {
                        _observableCollection.Add(item);
                    }
                }

                return new ReadOnlyObservableCollection<ClipboardDataViewModel>(_observableCollection);
            });
        }

        private void UpdateCollectionByStoreFilter()
        {
            WorkOnCollection(() =>
            {
                _mainCollection.RemoveWhere(n =>
                {
                    if (!IsAbleToBeStored(n))
                    {
                        if (IsAbleToBeShown(n))
                            _observableCollection.Remove(n);

                        return true;
                    }

                    return false;
                });

                return new ReadOnlyObservableCollection<ClipboardDataViewModel>(_observableCollection);
            });
        }

        private void WaitForTaskCollection()
            => TaskCollection.Task.Wait();

        private void WorkOnCollection(Func<ReadOnlyObservableCollection<ClipboardDataViewModel>> func)
        {
            WaitForTaskCollection();

            IsCollectionBusy = true;
            TaskCollection = new NotifyTaskCompletion<ReadOnlyObservableCollection<ClipboardDataViewModel>>(
                Task.Run(() => func()), () => IsCollectionBusy = false);
        }

        #endregion

    }
}
