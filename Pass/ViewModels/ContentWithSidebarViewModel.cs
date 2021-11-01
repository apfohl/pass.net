using Pass.Components.Binding;
using Pass.Components.ViewMapping;
using Pass.Views;

namespace Pass.ViewModels
{
    public interface ISidebar
    {
    }

    [View(typeof(ContentWithSidebarView))]
    public sealed class ContentWithSidebarViewModel : Bindable
    {
        public Bindable Content { get; }
        public ISidebar Sidebar { get; }

        public ContentWithSidebarViewModel(Bindable content, ISidebar sidebar)
        {
            Content = content;
            Sidebar = sidebar;
        }
    }
}