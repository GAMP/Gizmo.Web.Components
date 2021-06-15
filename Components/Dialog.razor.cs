using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Dialog : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Dialog()
        {
        }
        #endregion

        [Parameter]
        public RenderFragment DialogHeader { get; set; }

        [Parameter]
        public RenderFragment DialogBody { get; set; }

        [Parameter]
        public RenderFragment DialogFooter { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public bool IsModal { get; set; }

        protected Task OnClickDialogHandler(MouseEventArgs args)
        {
            if (IsModal)
                return Task.CompletedTask;

            IsOpen = false;
            return IsOpenChanged.InvokeAsync(IsOpen);
        }

        protected Task OnClickCloseButtonHandler(MouseEventArgs args)
        {
            IsOpen = false;
            return IsOpenChanged.InvokeAsync(IsOpen);
        }

        protected string ClassName => new ClassMapper()
                 .Add("g-dialog")
                 .If("g-dialog-open", () => IsOpen)
                 .AsString();
    }
}