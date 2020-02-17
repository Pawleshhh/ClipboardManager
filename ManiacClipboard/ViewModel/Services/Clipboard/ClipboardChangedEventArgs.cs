using ManiacClipboard.Model;
using System;

namespace ManiacClipboard.ViewModel
{
    /// <summary>
    /// Event arguments for the <see cref="IClipboardService.ClipboardChanged"/> event.
    /// </summary>
    public class ClipboardChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes the <see cref="ClipboardChangedEventArgs"/> instance.
        /// </summary>
        /// <param name="clipboardData">Clipboard data.</param>
        /// <exception cref="ArgumentNullException"/>
        public ClipboardChangedEventArgs(ClipboardData clipboardData)
        {
            if (clipboardData == null)
                throw new ArgumentNullException("clipboardData");

            ClipboardData = clipboardData;
        }

        /// <summary>
        /// Gets clipboard data.
        /// </summary>
        public ClipboardData ClipboardData { get; }
    }
}