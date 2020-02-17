namespace ManiacClipboard.Model
{
    /// <summary>
    /// Enum values that indicate certain clipboard data type.
    /// </summary>
    public enum ClipboardDataType
    {
        /// <summary>
        /// Unknown data.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Text data.
        /// </summary>
        Text,

        /// <summary>
        /// Text which contains path to specified file.
        /// </summary>
        FilePath,

        /// <summary>
        /// Collection of paths to specified files.
        /// </summary>
        FileList,

        /// <summary>
        /// Image data.
        /// </summary>
        Image,
    }
}