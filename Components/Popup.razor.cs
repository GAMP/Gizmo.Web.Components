using Gizmo.Web.Components.Infrastructure;
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

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public int MaximumHeight { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public bool Modal { get; set; }

        #endregion

        #region EVENTS

        protected Task OnClickPopupHandler(MouseEventArgs args)
        {
            if (Modal)
                return Task.CompletedTask;

            IsOpen = false;
            return IsOpenChanged.InvokeAsync(IsOpen);
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-popup")
                 .If("giz-popup-open", () => IsOpen)
                 .AsString();

        protected string StyleValue => new StyleMapper()
                 .If($"max-height: {@MaximumHeight}px", () => MaximumHeight > 0)
                 .AsString();

        #endregion

    }
}