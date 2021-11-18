using System.Threading.Tasks;

namespace Pass.Components.Dialog;

public interface IDialogPresenter
{
    Task Show(IDialog dialog);
}