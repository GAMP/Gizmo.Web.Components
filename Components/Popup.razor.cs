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
        public bool Open { get; set; }

        [Parameter]
        public EventCallback<bool> OpenChanged { get; set; }

        [Parameter]
        public bool Modal { get; set; }

        protected Task OnClickHandler(MouseEventArgs args)
        {
            if (Modal)
                return Task.CompletedTask;

            Open = false;
            return OpenChanged.InvokeAsync(Open);
        }

        protected string ClassName => new ClassMapper()
                 .Add("g-popup")
                 .If("g-popup-open", () => Open)
                 .AsString();
    }
}