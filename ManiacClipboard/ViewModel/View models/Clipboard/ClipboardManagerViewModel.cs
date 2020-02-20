﻿using ManiacClipboard.Model;
using System;
using System.Threading.Tasks;

namespace ManiacClipboard.ViewModel
{
    public class ClipboardManagerViewModel : NotifyPropertyChanges
    {

        #region Constructors

        public ClipboardManagerViewModel(IClipboardService clipboardService)
        {
            if (clipboardService == null)
                throw new ArgumentNullException("clipboardService");

            _clipboardService = clipboardService;

            _clipboardService.ClipboardChanged += _clipboardService_ClipboardChanged;

            ClipboardCollectionVM = new ClipboardCollectionViewModel();
        }

        #endregion

        #region Private fields

        private IClipboardService _clipboardService;

        private bool _isClipboardBusy;

        private bool _isClipboardEmpty;

        #endregion

        #region Properties

        public bool IsClipboardBusy
        {
            get => _isClipboardBusy;
            set
            {
                SetProperty(() => _isClipboardBusy == value,
                    () => _isClipboardBusy = value);
            }
        }

        public ClipboardCollectionViewModel ClipboardCollectionVM { get; }

        public bool IsMonitoring
        {
            get => _clipboardService.IsMonitoring;
            set
            {
                SetProperty(() => _clipboardService.IsMonitoring == value,
                    () =>
                    {
                        if (value)
                            _clipboardService.StartMonitoring();
                        else
                            _clipboardService.StopMonitoring();
                    });
            }
        }

        public bool AutoConvert
        {
            get => _clipboardService.AutoConvert;
            set
            {
                SetProperty(() => _clipboardService.AutoConvert == value,
                    () => _clipboardService.AutoConvert = value);
            }
        }

        public bool IsClipboardEmpty => _clipboardService.IsClipboardEmpty();

        public NotifyTaskCompletion ClipboardTask { get; private set; }

        #endregion

        #region Public methods

        public void AddCurrentData()
        {
            WorkOnClipboard(() =>
            {
                ClipboardData data = _clipboardService.GetClipboardData();
                if (data == null)
                    return;

                ClipboardDataViewModel dataVM = ClipboardDataViewModelFactory.Get(data);

                ClipboardCollectionVM.Add(dataVM);
            });
        }

        public void SetData(ClipboardDataViewModel dataVM)
        {
            ClipboardData data = ClipboardDataViewModel.GetModel(dataVM);

            WorkOnClipboard(_clipboardService.SetClipboardDataAsync(data));
        }

        public void ClearClipboard()
        {
            WorkOnClipboard(_clipboardService.ClearClipboardAsync());
        }

        public void SplitData<T>(CollectionClipboardDataViewModel<T> collectionVM)
        {
            if(ClipboardCollectionVM.Remove(collectionVM))
            {
                ClipboardCollectionVM.AddRange(collectionVM.Split());
            }
        }

        #endregion

        #region Private methods

        protected override void DisposeUnmanaged()
        {
            IsMonitoring = false;
            _clipboardService.Dispose();
        }

        private void WaitForCurrentTask()
        {
            ClipboardTask?.Task.Wait();
        }

        private void WorkOnClipboard(Action action)
        {
            WaitForCurrentTask();

            IsClipboardBusy = true;
            ClipboardTask = new NotifyTaskCompletion(Task.Run(() => action()),
                () => IsClipboardBusy = false);
        }

        private void WorkOnClipboard<TResult>(Func<TResult> func)
        {
            WaitForCurrentTask();

            IsClipboardBusy = true;
            ClipboardTask = new NotifyTaskCompletion<TResult>(Task.Run(() => func()),
                () => IsClipboardBusy = false);
        }

        private void WorkOnClipboard(Task task)
        {
            WaitForCurrentTask();

            IsClipboardBusy = true;
            ClipboardTask = new NotifyTaskCompletion(task, () => IsClipboardBusy = false);
        }

        private void WorkOnClipboard<TResult>(Task<TResult> task)
        {
            WaitForCurrentTask();

            IsClipboardBusy = true;
            ClipboardTask = new NotifyTaskCompletion<TResult>(task, () => IsClipboardBusy = false);
        }

        private bool GetResultFromTask<T>(ref T result)
        {
            if(ClipboardTask is NotifyTaskCompletion<T> task)
            {
                result = task.Result;
                return true;
            }

            result = default;
            return false;
        }

        private void _clipboardService_ClipboardChanged(object sender, ClipboardChangedEventArgs e)
        {
            ClipboardDataViewModel clipboardDataVM = ClipboardDataViewModelFactory.Get(e.ClipboardData);

            if (clipboardDataVM == null)
                return;

            ClipboardCollectionVM.Add(clipboardDataVM);
        }

        //private async void CheckIfClipboardIsEmpty()
        //{
        //    WorkOnClipboard(_clipboardService.IsClipboardEmptyAsync());
        //    await ClipboardTask.Task;

        //    bool result = true;
        //    if (!GetResultFromTask(ref result))
        //        SetProperty(() => _isClipboardEmpty == true,
        //            () => _isClipboardEmpty = true, "IsClipboardEmpty");
        //    else
        //        SetProperty(() => _isClipboardEmpty == result,
        //            () => _isClipboardEmpty = result, "IsClipboardEmpty");
        //}

        #endregion

    }
}
