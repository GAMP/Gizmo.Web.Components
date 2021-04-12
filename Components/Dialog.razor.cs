using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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
        public bool Open { get; set; }

        [Parameter]
        public bool Modal { get; set; }
        protected void OnClickHandler(MouseEventArgs args)
        {
            if (Modal)
                return;

            Open = false;
        }

        protected string ClassName => new ClassMapper()
                 .Add("g-dialog")
                 .If("g-dialog-open", () => Open)
                 .AsString();
    }
}
