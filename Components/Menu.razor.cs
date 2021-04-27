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
        public RenderFragment ChildContent { get; set; }

        protected string ClassName => new ClassMapper()
                 .Add("g-menu")
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("g-menu-container")
                 .Add("g-popup-bottom")
                 .Add("g-shadow-8")
                 .AsString();

        protected void OnClickMenuHandler(MouseEventArgs args)
        {
            IsOpen = true;
        }

        protected void OnClickOverlayHandler(MouseEventArgs args)
        {
            IsOpen = false;
        }

    }
}