using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Web.Components
{
    public partial class Menu : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Menu()
        {
        }
        #endregion

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string ClassName => new ClassMapper()
                 .Add("giz-menu")
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-menu-dropdown")
                 .Add("g-popup-bottom")
                 .Add("giz-elevation-2")
                 .AsString();

        protected void OnClickMenuHandler(MouseEventArgs args)
        {
            if (IsDisabled)
                return;

            IsOpen = true;
        }

        protected void OnClickOverlayHandler(MouseEventArgs args)
        {
            IsOpen = false;
        }

    }
}