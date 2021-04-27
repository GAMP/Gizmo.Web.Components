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
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public bool IsModal { get; set; }

        protected Task OnClickHandler(MouseEventArgs args)
        {
            if (IsModal)
                return Task.CompletedTask;

            IsOpen = false;
            return IsOpenChanged.InvokeAsync(IsOpen);
        }

        protected string ClassName => new ClassMapper()
                 .Add("g-dialog")
                 .If("g-dialog-open", () => IsOpen)
                 .AsString();
    }
}