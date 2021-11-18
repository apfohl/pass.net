using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Pass.Components.Threading;
using Pass.Views;

namespace Pass.Components.Dialog;

public sealed class DefaultDialogPresenter : IDialogPresenter
{
    private readonly Stack<Window> stack = new();
    private Window Parent => stack.Peek();

    public DefaultDialogPresenter(Window window) => stack.Push(window);

    public Task Show(IDialog dialog) =>
        Dispatcher.Dispatch(() => DialogView(dialog).ShowDialog(Parent, stack.Push));

    private DialogView DialogView(IDialog dialog)
    {
        var view = new DialogView { DataContext = dialog };
        dialog.Closed += (_, _) => Dispatcher.Dispatch(view.Close);
        view.Closing += (_, _) => stack.Pop();

        return view;
    }
}