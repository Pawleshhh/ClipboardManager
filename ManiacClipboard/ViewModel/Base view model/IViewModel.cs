namespace ManiacClipboard.ViewModel
{
    /// <summary>
    /// Defines interface for every view model.
    /// </summary>
    public interface IViewModel
    {
        /// <summary>
        /// Gets owner of this view model.
        /// </summary>
        IViewModel Owner { get; }
    }
}