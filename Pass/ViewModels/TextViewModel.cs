using System;
using System.Reactive.Linq;
using Pass.Components.Binding;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    [View(typeof(TextView))]
    public sealed class TextViewModel : Bindable, IContent, IDisposable
    {
        private readonly ReactiveProperty<string> content;
        private readonly IDisposable subscription;

        public string Content
        {
            get => content.Value;
            set => content.Value = value;
        }

        public TextViewModel(string content)
        {
            this.content = new ReactiveProperty<string>(content);
            subscription = this.content.Changed.Select(_ => nameof(Content)).Subscribe(OnPropertyChanged);
        }

        public void Dispose() => subscription.Dispose();
    }
}