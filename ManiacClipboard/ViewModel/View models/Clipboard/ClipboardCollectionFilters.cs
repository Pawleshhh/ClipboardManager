using System;

namespace ManiacClipboard.ViewModel
{
    [Flags]
    public enum ClipboardCollectionFilters
    {
        None = 0,
        Text = 1,
        FilePath = 2,
        FileList = 4,
        Image = 8,
        Unknown = 16,
        All = Text | FilePath | FileList | Image | Unknown,
    }
}
