using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Pass.Components.Commands;

public sealed class RelayCommand : CommandBase
{
    private readonly Func<Task> func;
    private readonly Func<bool> canExecute;

    public RelayCommand(Func<Task> func, Func<bool> canExecute)
        : this(func, canExecute, Observable.Never<Unit>())
    {
    }

    public RelayCommand(Func<Task> func, Func<bool> canExecute, IObservable<Unit> canExecuteChanged)
        : base(canExecuteChanged)
    {
        this.func = func;
        this.canExecute = canExecute;
    }

    protected override async void OnExecute(object parameter) => await func();

    protected override bool OnCanExecute(object parameter) => canExecute();
}