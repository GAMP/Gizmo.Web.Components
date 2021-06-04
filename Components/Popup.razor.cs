using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Popup : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Popup()
        {
        }
        #endregion

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }
        [Parameter]
        public string MaximumHeight { get; set; }
        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public bool Modal { get; set; }

        protected Task OnClickHandler(MouseEventArgs args)
        {
            if (Modal)
                return Task.CompletedTask;

            IsOpen = false;
            return IsOpenChanged.InvokeAsync(IsOpen);
        }

        protected string ClassName => new ClassMapper()
                 .Add("g-popup")
                 .If("g-popup-open", () => IsOpen)
                 .AsString();
    }
}