using ManiacClipboard.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ManiacClipboard.ViewModel
{
    /// <summary>
    /// Provides functions to work with the Windows' clipboard.
    /// </summary>
    internal sealed class WindowsClipboardService : IClipboardService
    {
        #region Constructors

        /// <summary>
        /// Initializes new instance of the <see cref="WindowsClipboardService"/> class.
        /// </summary>
        /// <param name="window">Window that will be getting notified about clipboard's updates.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public WindowsClipboardService(Window window)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            RegisterToClipboardChain(window);
        }

        #endregion Constructors

        #region Private fields

        private const int WM_CLIPBOARDUPDATE = 0x031D;

        private const int MAX_CLIPBOARD_ACCESS = 20;

        private const int WAIT_TIME = 50;

        private HwndSource _hWndSource;

        #endregion Private fields

        #region Properties

        /// <summary>
        /// <see cref="IClipboardService.AutoConvert"/>.
        /// </summary>
        public bool AutoConvert { get; set; } = true;

        /// <summary>
        /// <see cref="IClipboardService.IsMonitoring"/>.
        /// </summary>
        public bool IsMonitoring { get; private set; }

        #endregion Properties

        #region Events

        /// <summary>
        /// <see cref="IClipboardService.ClipboardChanged"/>.
        /// </summary>
        public event EventHandler<ClipboardChangedEventArgs> ClipboardChanged;

        private void OnClipboardChanged(ClipboardData data)
        {
            ClipboardChanged?.Invoke(this, new ClipboardChangedEventArgs(data));
        }

        #endregion Events

        #region Imported functions

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        #endregion Imported functions

        #region Public methods

        /// <summary>
        /// <see cref="IClipboardService.GetClipboardData"/>.
        /// </summary>
        public ClipboardData GetClipboardData()
        {
            ThrowIfDisposed();

            WindowsClipboardDataType type = WindowsClipboardDataType.None;

            (object data, string[] formats) = WorkOnClipboardSafe(() =>
            {
                type = GetClipboardDataTypeNotSafe();

                return GetDataByType(type);
            }, (null, null));

            return ClipboardDataFactory(data, type, DateTime.Now, null, formats);
        }

        /// <summary>
        /// <see cref="IClipboardService.GetClipboardDataAsync"/>.
        /// </summary>
        public Task<ClipboardData> GetClipboardDataAsync()
        {
            return Task.Run(() => GetClipboardData());
        }

        /// <summary>
        /// <see cref="IClipboardService.SetClipboardData"/>.
        /// </summary>
        public void SetClipboardData([AllowNull] ClipboardData data)
        {
            ThrowIfDisposed();
            if (data == null)
            {
                ClearClipboard();
                return;
            }

            Action action;
            switch(data.DataType)
            {
                case ClipboardDataType.Text:
                    action = () => Clipboard.SetText(((TextClipboardData)data).Data);
                    break;
                case ClipboardDataType.FilePath:
                    action = () => Clipboard.SetFileDropList(GetFileDropList(((PathClipboardData)data).Data));
                    break;
                case ClipboardDataType.FileList:
                    action = () => Clipboard.SetFileDropList(
                        GetFileDropList(((FileListClipboardData)data).Data.Select(n => n.Key).ToArray()));
                    break;
                case ClipboardDataType.Image:
                    action = () => Clipboard.SetImage(GetBitmap(((ImageClipboardData)data).Data));
                    break;
                case ClipboardDataType.Unknown:
                    var unknownData = (UnknownClipboardData)data;
                    action = () => Clipboard.SetDataObject(GetUnknownData(unknownData.Data, unknownData.GetFormats()));
                    break;
                default:
                    action = () => Clipboard.SetDataObject(GetUnknownData(data.Data, null));
                    break;
            }

            #region
            //if (data is TextClipboardData text)
            //{
            //    if (data is PathClipboardData)
            //    {
            //        action = () => Clipboard.SetFileDropList(GetFileDropList(text.Data));
            //    }
            //    else
            //        action = () => Clipboard.SetText(text.Data);
            //}
            //else if (data is FileListClipboardData fileDropList)
            //{
            //    action = () => Clipboard.SetFileDropList(
            //        GetFileDropList(fileDropList.Data.Select(n => n.Key).ToArray()));
            //}
            //else if (data is ImageClipboardData image)
            //{
            //    action = () => Clipboard.SetImage(GetBitmap(image.Data));
            //}
            //else if (data is AudioClipboardData audio)
            //{
            //    action = () => Clipboard.SetAudio(audio.Data);
            //}
            //else if (data is UnknownClipboardData unknown)
            //{
            //    action = () => Clipboard.SetDataObject(GetUnknownData(unknown.Data, unknown.GetFormats()));
            //}
            //else
            //{
            //    action = () => Clipboard.SetDataObject(GetUnknownData(data.Data, null));
            //}
            #endregion

            WorkOnClipboardSafe(action);
        }

        /// <summary>
        /// <see cref="IClipboardService.SetClipboardDataAsync"/>.
        /// </summary>
        public Task SetClipboardDataAsync([AllowNull] ClipboardData data)
        {
            return Task.Run(() => SetClipboardData(data));
        }

        /// <summary>
        /// <see cref="IClipboardService.GetClipboardDataType"/>.
        /// </summary>
        public WindowsClipboardDataType GetClipboardDataType()
        {
            ThrowIfDisposed();

            return WorkOnClipboardSafe(() =>
            {
                return GetClipboardDataTypeNotSafe();
            }, WindowsClipboardDataType.None);
        }

        /// <summary>
        /// <see cref="IClipboardService.GetClipboardDataTypeAsync"/>.
        /// </summary>
        public Task<WindowsClipboardDataType> GetClipboardDataTypeAsync()
        {
            return Task.Run(() => GetClipboardDataType());
        }

        /// <summary>
        /// <see cref="IClipboardService.ClearClipboard"/>.
        /// </summary>
        public void ClearClipboard()
        {
            ThrowIfDisposed();
            WorkOnClipboardSafe(() => Clipboard.Clear());
        }

        /// <summary>
        /// <see cref="IClipboardService.ClearClipboardAsync"/>.
        /// </summary>
        public Task ClearClipboardAsync()
        {
            return Task.Run(() => ClearClipboard());
        }

        /// <summary>
        /// <see cref="IClipboardService.IsClipboardEmpty"/>.
        /// </summary>
        public bool IsClipboardEmpty()
        {
            ThrowIfDisposed();
            return WorkOnClipboardSafe(() => Clipboard.GetDataObject() == null, true);
        }

        /// <summary>
        /// <see cref="IClipboardService.IsClipboardEmptyAsync"/>.
        /// </summary>
        public Task<bool> IsClipboardEmptyAsync()
        {
            return Task.Run(() => IsClipboardEmpty());
        }

        /// <summary>
        /// <see cref="IClipboardService.StartMonitoring"/>.
        /// </summary>
        public void StartMonitoring()
        {
            IsMonitoring = true;
        }

        /// <summary>
        /// <see cref="IClipboardService.StopMonitoring"/>.
        /// </summary>
        public void StopMonitoring()
        {
            IsMonitoring = false;
        }

        #endregion Public methods

        #region Private methods

        private void RegisterToClipboardChain(Window window)
        {
            WindowInteropHelper wih = new WindowInteropHelper(window);

            _hWndSource = HwndSource.FromHwnd(wih.EnsureHandle());
            _hWndSource.AddHook(WndProc);

            bool result = AddClipboardFormatListener(_hWndSource.Handle);

            if (!result)
                throw new InvalidOperationException("Windows clipboard manager could not register to the clipboard chain.");
        }

        private void UnregisterFromClipboardChain()
        {
            RemoveClipboardFormatListener(_hWndSource.Handle);
        }

        private int _clipboardUpdateCounter = 0;

        private int ClipboardUpdateCounter
        {
            get
            {
                async void setClipboardUpdateCounter()
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(200);
                        ClipboardUpdateCounter = 0;
                    }
                    );
                }
                setClipboardUpdateCounter();
                return _clipboardUpdateCounter;
            }
            set
            {
                _clipboardUpdateCounter = value;
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_CLIPBOARDUPDATE:

                    if (IsMonitoring && ClipboardUpdateCounter < 1)
                    {
                        OnClipboardChanged(GetClipboardDataWithSource());
                        ClipboardUpdateCounter++;
                    }

                    break;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets currently stored data in the clipboard assuming the foreground window is the source.
        /// </summary>
        private ClipboardData GetClipboardDataWithSource()
        {
            ClipboardSource source = null;
            WindowsClipboardDataType type = WindowsClipboardDataType.None;
            Thread thread = new Thread(() => source = GetClipboardSource());
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            (object data, string[] formats) = WorkOnClipboardSafe(() =>
            {
                type = GetClipboardDataTypeNotSafe();

                return GetDataByType(type);
            }, (null, null));

            thread.Join();

            return ClipboardDataFactory(data, type, DateTime.Now, source, formats);
        }

        private WindowsClipboardDataType GetClipboardDataTypeNotSafe()
        {
            if (Clipboard.ContainsText()) return WindowsClipboardDataType.Text;
            if (Clipboard.ContainsFileDropList()) return WindowsClipboardDataType.FileList;
            if (Clipboard.ContainsImage()) return WindowsClipboardDataType.Image;
            if (Clipboard.ContainsAudio()) return WindowsClipboardDataType.Audio;

            return WindowsClipboardDataType.None;
        }

        private ClipboardData ClipboardDataFactory(object data, WindowsClipboardDataType type, DateTime copyTime, ClipboardSource source = null, params string[] formats)
        {
            switch (type)
            {
                case WindowsClipboardDataType.Text:
                    string text = (string)data;
                    if(AutoConvert)
                    {
                        if (Directory.Exists(text))
                            return new PathClipboardData(text, true, copyTime, source);
                        else if (File.Exists(text))
                            return new PathClipboardData(text, false, copyTime, source);
                    }

                    return new TextClipboardData((string)data, copyTime, source);

                case WindowsClipboardDataType.FileList:
                    StringCollection collection = (StringCollection)data;
                    if (collection.Count == 1)
                        return new PathClipboardData(collection[0], Directory.Exists(collection[0]), copyTime, source);
                    else
                        return new FileListClipboardData(collection.Cast<string>().
                            Select(n => new KeyValuePair<string, bool>(n, Directory.Exists(n))).
                            ToArray(), copyTime, source);

                case WindowsClipboardDataType.Image:
                    MemoryStream bmp = new MemoryStream();

                    BitmapEncoder enc = new BmpBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create((BitmapSource)data));
                    enc.Save(bmp);
                    return new ImageClipboardData(bmp, copyTime, source);

                //case ClipboardDataType.Audio:
                //    return new AudioClipboardData((Stream)data, copyTime, source);

                default:
                    return new UnknownClipboardData(data, formats, copyTime, source);
            }
        }

        private (object data, string[] formats) GetDataByType(WindowsClipboardDataType type)
        {
            switch (type)
            {
                case WindowsClipboardDataType.Text:
                    return (Clipboard.GetText(), null);

                case WindowsClipboardDataType.FileList:
                    return (Clipboard.GetFileDropList(), null);

                case WindowsClipboardDataType.Image:
                    var img = Clipboard.GetImage();
                    img.Freeze();
                    return (img, null);

                case WindowsClipboardDataType.Audio:
                    return (Clipboard.GetAudioStream(), null);

                default: //Unknown
                    IDataObject dataObject = Clipboard.GetDataObject();
                    string[] dataFormats = dataObject.GetFormats();

                    if (dataFormats == null || dataFormats.Length == 0)
                        return (null, null);

                    return (dataObject.GetData(dataFormats[0]), dataFormats);
            }
        }

        private ClipboardSource GetClipboardSource()
        {
            try
            {
                IntPtr hwnd = GetForegroundWindow();

                if (hwnd == IntPtr.Zero)
                    return null;

                uint pID;
                GetWindowThreadProcessId(hwnd, out pID);
                Process process = Process.GetProcessById((int)pID);

                string appName = process.ProcessName;
                string path = process.MainModule.FileName;

                return new ClipboardSource(appName, path);
            }
            catch
            {
                return null;
            }
        }

        private BitmapImage GetBitmap(MemoryStream stream)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

        private StringCollection GetFileDropList(params string[] paths)
        {
            StringCollection collection = new StringCollection();
            if (paths == null || paths.Length == 0)
                return collection;

            collection.AddRange(paths);
            return collection;
        }

        private DataObject GetUnknownData(object data, string[] formats)
        {
            if (formats == null || formats.Length == 0)
                return new DataObject(data.GetType(), data);

            return new DataObject(formats[0], data);
        }

        private TResult WorkOnClipboardSafe<TResult>(Func<TResult> func, TResult defaultValue)
        {
            TResult result = defaultValue;

            Thread thread = new Thread(() =>
            {
                int counter = 0;

                while (counter < MAX_CLIPBOARD_ACCESS)
                {
                    try
                    {
                        result = func();
                        break;
                    }
                    catch (ExternalException)
                    {
                        counter++;
                        Thread.Sleep(WAIT_TIME);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("When working with clipboard not external exception occurred.", ex);
                    }
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return result;
        }

        private void WorkOnClipboardSafe(Action action)
        {
            Thread thread = new Thread(() =>
            {
                int counter = 0;

                while (counter < MAX_CLIPBOARD_ACCESS)
                {
                    try
                    {
                        action();
                        break;
                    }
                    catch (ExternalException)
                    {
                        counter++;
                        Thread.Sleep(WAIT_TIME);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("When working with clipboard not external exception occurred.", ex);
                    }
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        #endregion Private methods

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        ~WindowsClipboardService()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                StopMonitoring();
                UnregisterFromClipboardChain();
                _hWndSource.Dispose();

                disposedValue = true;
            }
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (disposedValue)
                throw new ObjectDisposedException("WindowsClipboardManager");
        }

        #endregion IDisposable Support
    }
}