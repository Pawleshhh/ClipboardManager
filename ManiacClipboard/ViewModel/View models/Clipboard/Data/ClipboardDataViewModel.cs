
using ManiacClipboard.Model;
using System;

namespace ManiacClipboard.ViewModel
{
    public abstract class ClipboardDataViewModel : NotifyPropertyChanges
    {

        #region Constructors

        public ClipboardDataViewModel(ClipboardData clipboardData)
        {
            _clipboardData = clipboardData;
        }

        #endregion

        #region Private fields

        private readonly ClipboardData _clipboardData;

        #endregion

        #region Properties

        public object Data => _clipboardData.Data;

        public ClipboardDataType DataType => _clipboardData.DataType;

        public DateTime CopyTime => _clipboardData.CopyTime;

        public ClipboardSource Source => _clipboardData.Source;

        public string TagName
        {
            get => _clipboardData.TagName;
            set => SetProperty(() => _clipboardData.TagName == value,
                                    () => _clipboardData.TagName = value);
        }

        public bool KeepThat
        {
            get => _clipboardData.KeepThat;
            set => SetProperty(() => _clipboardData.KeepThat == value,
                                    () => _clipboardData.KeepThat = value);
        }

        #endregion

        #region Public methods

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if(obj is ClipboardDataViewModel data)
                return _clipboardData.Equals(data._clipboardData);

            return false;
        }

        public override int GetHashCode()
            => _clipboardData.GetHashCode();

        public override string ToString()
            => _clipboardData.ToString();

        protected override void DisposeUnmanaged()
        {
            _clipboardData.Dispose();
        }

        public static ClipboardData GetModel(ClipboardDataViewModel clipboardDataVM)
            => clipboardDataVM._clipboardData;

        #endregion

        #region Private methods

        #endregion
    }

    public abstract class ClipboardDataViewModel<T> : ClipboardDataViewModel
    {
        #region Constructors

        protected ClipboardDataViewModel(ClipboardData<T> clipboardData) : base(clipboardData)
        {
            Data = clipboardData.Data;
        }

        #endregion

        #region Properties

        public new T Data { get; }

        #endregion
    }
}
