using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Pass.Components.Commands;

public abstract class CommandBase : ICommand, IDisposable
{
    private readonly IDisposable canExecuteChanged;

    protected CommandBase() : this(Observable.Never<Unit>())
    {
    }

    protected CommandBase(IObservable<Unit> canExecuteChanged) =>
        this.canExecuteChanged = canExecuteChanged.Subscribe(_ => OnCanExecuteChanged());

    public bool CanExecute(object parameter) => OnCanExecute(parameter);

    public void Execute(object parameter) => OnExecute(parameter);

    public event EventHandler CanExecuteChanged;

    protected abstract void OnExecute(object parameter);

    protected virtual bool OnCanExecute(object parameter) => true;

    protected void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CommandBase() => Dispose(false);

    protected void Dispose(bool disposing)
    {
        if (disposing) canExecuteChanged?.Dispose();
    }
}