using GalaSoft.MvvmLight.Helpers;
using System;
using System.Threading.Tasks;

namespace GalaSoft.MvvmLight.Command
{
    public class AsyncRelayCommand : RelayCommand
    {
        readonly WeakFunc<Task> asyncExecute;
        //readonly Action execute;

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

            this.asyncExecute = new WeakFunc<Task>(asyncExecute);
            //this.execute = execute;
        }

        public Task ExecuteAsync() => asyncExecute.Execute();

        public override void Execute(object parameter) => asyncExecute.Execute();
    }

    public class AsyncRelayCommand<T> : RelayCommand<T>
    {
        readonly WeakFunc<T, Task> asyncExecute;
        //readonly Action<T> execute;

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

            this.asyncExecute = new WeakFunc<T, Task>(asyncExecute);
            //this.execute = execute;
        }

        public Task ExecuteAsync(T param) => asyncExecute.Execute(param);

        public override void Execute(object parameter) => asyncExecute.Execute((T)parameter);
    }
}