using System;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace Pass.Components.Dialog;

public static class WindowExtensions
{
    public static Task ShowDialog(this Window window, Window parent, Action<Window> onShowDialog)
    {
        onShowDialog(window);
        return window.ShowDialog(parent);
    }
}