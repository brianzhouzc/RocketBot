using System;
using System.Threading.Tasks;

namespace GalaSoft.MvvmLight.Command
{
    public class AsyncRelayCommand : RelayCommand
    {
        readonly Func<Task> asyncExecute;
        readonly Action execute;

        public AsyncRelayCommand(Func<Task> asyncExecute)
            : this(asyncExecute, null)
        {
        }

        public AsyncRelayCommand(Func<Task> asyncExecute, Func<bool> canExecute)
            : this(asyncExecute, () => asyncExecute?.Invoke(), canExecute)
        {
        }

        AsyncRelayCommand(Func<Task> asyncExecute, Action execute, Func<bool> canExecute)
            : base(execute, canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            if (asyncExecute == null)
                throw new ArgumentNullException(nameof(asyncExecute));

            this.asyncExecute = asyncExecute;
            this.execute = execute;
        }

        public Task ExecuteAsync() => asyncExecute();
    }

    public class AsyncRelayCommand<T> : RelayCommand<T>
    {
        readonly Func<T, Task> asyncExecute;
        readonly Action<T> execute;

        public AsyncRelayCommand(Func<T, Task> asyncExecute)
            : this(asyncExecute, null)
        {
        }
        public AsyncRelayCommand(Func<T, Task> asyncExecute, Func<T, bool> canExecute)
            : this(asyncExecute, x => asyncExecute?.Invoke(x), canExecute)
        {
        }

        AsyncRelayCommand(Func<T, Task> asyncExecute, Action<T> execute, Func<T, bool> canExecute)
            : base(execute, canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            if (asyncExecute == null)
                throw new ArgumentNullException(nameof(asyncExecute));

            this.asyncExecute = asyncExecute;
            this.execute = execute;
        }

        public Task ExecuteAsync(T param) => asyncExecute(param);
    }
}