using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Overlay : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Overlay()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public bool Visible { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        #endregion

        #region EVENTS

        protected Task OnClickOverlayHandler(MouseEventArgs args)
        {
            return OnClick.InvokeAsync(args);
        }

        protected Task OnContextMenuHandler(MouseEventArgs args)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-overlay")
                 .If("giz-overlay--visible", () => Visible)
                 .AsString();

        #endregion

    }
}