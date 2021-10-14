using System;
using System.Threading.Tasks;

namespace Pass.Components.Commands
{
    public sealed class RelayCommand : CommandBase
    {
        private readonly Func<Task> func;
        private readonly Func<bool> canExecute;

        public RelayCommand(Func<Task> func, Func<bool> canExecute)
        {
            this.func = func;
            this.canExecute = canExecute;
        }

        protected override async void OnExecute(object parameter) => await func();

        protected override bool OnCanExecute(object parameter) => canExecute();

    }
}