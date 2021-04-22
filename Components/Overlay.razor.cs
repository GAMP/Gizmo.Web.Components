using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Web.Components
{
    public partial class Overlay : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Overlay()
        {
        }
        #endregion

        [Parameter]
        public bool Visible { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected void OnClickHandler(MouseEventArgs args)
        {
            OnClick.InvokeAsync(args);
        }

        protected string ClassName => new ClassMapper()
                 .Add("g-overlay")
                 .If("g-overlay-visible", () => Visible)
                 .AsString();

    }
}