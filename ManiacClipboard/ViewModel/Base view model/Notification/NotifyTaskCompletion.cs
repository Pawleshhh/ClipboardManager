using System;
using System.Threading.Tasks;

namespace ManiacClipboard.ViewModel
{

    public class NotifyTaskCompletion : NotifyPropertyChanges
    {

        public NotifyTaskCompletion(Task task)
        {
            Task = task;
            if (!task.IsCompleted)
            {
                var a = WatchTaskAsync(task);
            }
        }

        public NotifyTaskCompletion(Task task, Action finished) : this(task)
        {
            this.finished = finished;
        }

        protected readonly Action finished;

        public Task Task { get; }

        public TaskStatus Status { get { return Task.Status; } }

        public bool IsCompleted { get { return Task.IsCompleted; } }

        public bool IsNotCompleted { get { return !Task.IsCompleted; } }

        public bool IsSuccessfullyCompleted
        {
            get
            {
                return Task.Status == TaskStatus.RanToCompletion;
            }
        }

        public bool IsCanceled { get { return Task.IsCanceled; } }

        public bool IsFaulted { get { return Task.IsFaulted; } }

        public AggregateException Exception { get { return Task.Exception; } }

        public Exception InnerException
        {
            get
            {
                return (Exception == null) ?
                    null : Exception.InnerException;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return (InnerException == null) ?
                    null : InnerException.Message;
            }
        }

        protected virtual async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch
            {
            }

            OnPropertyChanged("Status", "IsCompleted", "IsNotCompleted");

            if (task.IsCanceled)
            {
                OnPropertyChanged("IsCanceled");
            }
            else if (task.IsFaulted)
            {
                OnPropertyChanged("IsFaulted", "Exception", "InnerException", "ErrorMessage");
            }
            else
            {
                OnPropertyChanged("IsSuccessfullyCompleted");
            }

            finished?.Invoke();
        }

    }


    /// <summary>
    /// Code from https://msdn.microsoft.com/en-us/magazine/dn605875.aspx?f=255&MSPPError=-2147217396.
    /// </summary>
    public class NotifyTaskCompletion<TResult> : NotifyTaskCompletion
    {
        public NotifyTaskCompletion(Task<TResult> task) : base(task)
        {
            Task = task;
        }

        public NotifyTaskCompletion(Task<TResult> task, Action finished) : base(task, finished)
        {
            Task = task;
        }

        public new Task<TResult> Task { get; }

        public TResult Result
        {
            get
            {
                return (Task.Status == TaskStatus.RanToCompletion) ?
                    Task.Result : default;
            }
        }

        protected override async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch
            {
            }

            OnPropertyChanged("Status", "IsCompleted", "IsNotCompleted");

            if (task.IsCanceled)
            {
                OnPropertyChanged("IsCanceled");
            }
            else if (task.IsFaulted)
            {
                OnPropertyChanged("IsFaulted", "Exception", "InnerException", "ErrorMessage");
            }
            else
            {
                OnPropertyChanged("IsSuccessfullyCompleted", "Result");
            }

            finished?.Invoke();
        }
    }
}