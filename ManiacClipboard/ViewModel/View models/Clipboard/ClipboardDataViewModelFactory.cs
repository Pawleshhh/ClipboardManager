using ManiacClipboard.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManiacClipboard.ViewModel
{
    public static class ClipboardDataViewModelFactory
    {

        public static ClipboardDataViewModel Get(ClipboardData data)
        {
            if (data == null)
                return null;

            switch(data.DataType)
            {
                case ClipboardDataType.Text:
                    return new TextClipboardDataViewModel((TextClipboardData)data);
                case ClipboardDataType.FilePath:
                    return new PathClipboardDataViewModel((PathClipboardData)data);
                case ClipboardDataType.FileList:
                    return new FileListClipboardDataViewModel((FileListClipboardData)data);
                case ClipboardDataType.Image:
                    return new ImageClipboardDataViewModel((ImageClipboardData)data);
                case ClipboardDataType.Unknown:
                    return new UnknownClipboardDataViewModel((UnknownClipboardData)data);
                default:
                    return null;
            }
        }

    }
}
