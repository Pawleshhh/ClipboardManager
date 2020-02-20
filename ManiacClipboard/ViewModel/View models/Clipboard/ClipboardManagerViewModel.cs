using ManiacClipboard.Model;
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

        #endregion

        #region Properties

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

        public Task CurrentTask { get; private set; }

        #endregion

        #region Public methods

        public async void AddCurrentData()
        {
            WaitForCurrentTask();
            CurrentTask = _clipboardService.GetClipboardDataAsync();

            var data = GetResultFromTask<ClipboardData>();

            if (data == null)
                return;

            ClipboardDataViewModel dataVM = ClipboardDataViewModelFactory.Get(data);

            ClipboardCollectionVM.Add(dataVM);
        }

        public async void SetData(ClipboardDataViewModel dataVM)
        {
            ClipboardData data = ClipboardDataViewModel.GetModel(dataVM);

            await _clipboardService.SetClipboardDataAsync(data);
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

        private void WaitForCurrentTask()
        {
            CurrentTask?.Wait();
        }

        private async void WorkOnTask(Task task)
        {
            WaitForCurrentTask();
            CurrentTask = task;
            await CurrentTask;
        }

        private T GetResultFromTask<T>()
        {
            return ((Task<T>)CurrentTask).Result;
        }

        private void _clipboardService_ClipboardChanged(object sender, ClipboardChangedEventArgs e)
        {
            ClipboardDataViewModel clipboardDataVM = ClipboardDataViewModelFactory.Get(e.ClipboardData);

            if (clipboardDataVM == null)
                return;

            ClipboardCollectionVM.Add(clipboardDataVM);
        }

        #endregion

    }
}
