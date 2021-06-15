using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using static Gizmo.Web.Components.Button;
using static Gizmo.Web.Components.List;

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
        public ButtonVariants Variant { get; set; } = ButtonVariants.Outline;

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public RenderFragment Activator { get; set; }

        [Parameter]
        public ListDirection Direction { get; set; } = ListDirection.Right;

        [Parameter]
        public ButtonSize Size { get; set; } = ButtonSize.Medium;

        protected string ClassName => new ClassMapper()
                 .Add("giz-menu")
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-menu-dropdown")
                 .Add("giz-popup-bottom")                
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