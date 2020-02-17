using System;

namespace ManiacClipboard.Model
{
    /// <summary>
    /// Represents the interface of data that can be stored on the clipboard.
    /// </summary>
    public interface IClipboardData : IDisposable
    {
        /// <summary>
        /// Gets stored data.
        /// </summary>
        object Data { get; }

        /// <summary>
        /// Gets type of stored data.
        /// </summary>
        ClipboardDataType DataType { get; }

        /// <summary>
        /// Gets date and time when data was stored.
        /// </summary>
        DateTime CopyTime { get; }

        /// <summary>
        /// Gets the source where the stored data comes from.
        /// </summary>
        ClipboardSource Source { get; }

        /// <summary>
        /// Gets or sets the tag name of stored data.
        /// </summary>
        string TagName { get; set; }

        /// <summary>
        /// Gets or sets whether the stored data is supposed to be kept or not.
        /// </summary>
        bool KeepThat { get; set; }
    }

    public interface IClipboardData<T> : IClipboardData
    {
        /// <summary>
        /// Gets stored data.
        /// </summary>
        new T Data { get; }
    }
}