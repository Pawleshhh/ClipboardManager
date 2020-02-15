using System;

namespace ManiacClipboard.Model
{
    /// <summary>
    /// Text type of clipboard data.
    /// </summary>
    public class TextClipboardData : ClipboardData<string>, IEquatable<TextClipboardData>
    {
        public TextClipboardData(string data) : base(data, ClipboardDataType.Text)
        {
        }

        public TextClipboardData(string data, DateTime copyTime) : base(data, ClipboardDataType.Text, copyTime)
        {
        }

        public TextClipboardData(string data, ClipboardSource source) : base(data, ClipboardDataType.Text, source)
        {
        }

        public TextClipboardData(string data, DateTime copyTime, ClipboardSource source) : base(data, ClipboardDataType.Text, copyTime, source)
        {
        }

        public bool Equals(TextClipboardData other)
            => other != null && Data.Equals(other.Data);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is TextClipboardData text)
                return Equals(text);

            return false;
        }

        public override int GetHashCode() => Data.GetHashCode() * 17;
    }
}
