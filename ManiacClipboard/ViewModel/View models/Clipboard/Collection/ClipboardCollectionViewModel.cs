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

        public ClipboardCollectionViewModel(int limit) : this()
        {
            if (limit < 1)
                throw new ArgumentException("limit cannot be less than 1.");

            _limit = limit;
        }
        #endregion

        #region Private fields

        private int _limit = 100;

        private bool _alwaysFitToLimit = true;

        private bool _isAscendingOrder;

        private ClipboardCollectionFilters _showTypeOfClipboardData = ClipboardCollectionFilters.All;

        private ClipboardCollectionFilters _storeTypeOfClipboardData = ClipboardCollectionFilters.All;

        private bool _isCollectionBusy;

        private readonly HashSet<ClipboardDataViewModel> _mainCollection;

        private ObservableCollection<ClipboardDataViewModel> _observableCollection;

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

        public int Limit
        {
            get => _limit;
            set
            {
                if (value < 1)
                    throw new ArgumentException("Limit cannot be less than 1.");

                SetProperty(() => _limit == value, () => _limit = value);

                if(AlwaysFitToLimit)
                    FitToLimit();
            }
        }

        public bool AlwaysFitToLimit
        {
            get => _alwaysFitToLimit;
            set
            {
                SetProperty(() => _alwaysFitToLimit == value,
                    () => _alwaysFitToLimit = value);

                if (value)
                    FitToLimit();
            }
        }

        public bool IsAscendingOrder
        {
            get => _isAscendingOrder;
            set
            {
                SetProperty(() => _isAscendingOrder == value,
                    () => _isAscendingOrder = value);

                Sort();
            }
        }

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
                if (IsAbleToBeShown(data))
                    AddSorted(data);

                if(AlwaysFitToLimit)
                    FitToLimit();
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
                data.Dispose();

                if(AlwaysFitToLimit)
                    FitToLimit();
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
                            AddSorted(item);
                    }
                }

                if(AlwaysFitToLimit)
                    _FitToLimit();
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
                    {
                        _observableCollection.Remove(item);
                        item.Dispose();
                    }
                }

                if (AlwaysFitToLimit)
                    _FitToLimit();
                return new ReadOnlyObservableCollection<ClipboardDataViewModel>(_observableCollection);
            });
        }

        public void Clear()
        {
            WaitForTaskCollection();

            foreach (var item in _mainCollection)
                item.Dispose();

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

        public void FitToLimit()
        {
            WaitForTaskCollection();

            int reqSpace = _observableCollection.Count - Limit;

            if (reqSpace <= 0)
                return;

            WorkOnCollection(() =>
            {
                _FitToLimit();

                return new ReadOnlyObservableCollection<ClipboardDataViewModel>(_observableCollection);
            });
        }

        #endregion

        #region Private methods

        private void _FitToLimit()
        {
            int reqSpace = _observableCollection.Count - Limit;

            if (reqSpace <= 0)
                return;

            List<ClipboardDataViewModel> itemsToRemove = new List<ClipboardDataViewModel>();

            foreach (var item in _mainCollection.OrderBy(n => n.CopyTime))
            {
                if (!item.KeepThat)
                    itemsToRemove.Add(item);

                if (itemsToRemove.Count == reqSpace)
                    break;
            }

            foreach (var item in itemsToRemove)
            {
                _mainCollection.Remove(item);
                _observableCollection.Remove(item);
                item.Dispose();
            }

        }

        private void AddSorted(ClipboardDataViewModel data)
        {
            int i;

            if(IsAscendingOrder)
            {
                i = _observableCollection.Count - 1;

                while (i >= 0 && _observableCollection[i].CopyTime.CompareTo(data.CopyTime) < 0)
                    i--;
            }
            else
            {
                i = 0;

                while (i < _observableCollection.Count &&
                    _observableCollection[i].CopyTime.CompareTo(data.CopyTime) > 0)
                    i++;
            }

            if (i == _observableCollection.Count || i < 0)
                _observableCollection.Add(data);
            else
                _observableCollection.Insert(i, data);
        }

        private void Sort()
        {
            WorkOnCollection(() =>
            {
                if (IsAscendingOrder)
                    _observableCollection = new ObservableCollection<ClipboardDataViewModel>(
                        _observableCollection.OrderBy(n => n.CopyTime));
                else
                    _observableCollection = new ObservableCollection<ClipboardDataViewModel>(
                        _observableCollection.OrderByDescending(n => n.CopyTime));

                return new ReadOnlyObservableCollection<ClipboardDataViewModel>(_observableCollection);
            });
        }

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

                if (AlwaysFitToLimit)
                    _FitToLimit();
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

                        n.Dispose();

                        return true;
                    }

                    return false;
                });

                if (AlwaysFitToLimit)
                    _FitToLimit();
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
