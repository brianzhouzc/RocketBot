using System;
using System.Threading.Tasks;

namespace GalaSoft.MvvmLight.Command
{
    public class AsyncRelayCommand : RelayCommand
    {
        private readonly Func<Task> asyncExecute;
        private readonly Action execute;

        public AsyncRelayCommand(Func<Task> asyncExecute)
            : this(asyncExecute, () => asyncExecute())
        {
        }

        private AsyncRelayCommand(Func<Task> asyncExecute, Action execute)
            : base(execute)
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
        private readonly Func<T, Task> asyncExecute;
        private readonly Action<T> execute;

        public AsyncRelayCommand(Func<T, Task> asyncExecute)
            : this(asyncExecute, x => asyncExecute(x))
        {
        }

        private AsyncRelayCommand(Func<T, Task> asyncExecute, Action<T> execute)
            : base(execute)
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