using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Menu : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Menu()
        {
        }
        #endregion

        #region PROPERTIES

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
        public ListDirections Direction { get; set; } = ListDirections.Right;

        [Parameter]
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        #endregion

        #region EVENTS

        protected Task OnClickMenuHandler(MouseEventArgs args)
        {
            if (!IsDisabled)
                IsOpen = true;

            return Task.CompletedTask;
        }

        protected Task OnClickOverlayHandler(MouseEventArgs args)
        {
            IsOpen = false;

            return Task.CompletedTask;
        }

        #endregion

        #region METHODS

        internal void Close()
        {
            IsOpen = false;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-menu")
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-menu-dropdown")
                 .Add("giz-popup-bottom")
                 .AsString();

        #endregion

    }
}