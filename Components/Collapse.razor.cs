using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class Collapse : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Collapse()
        {
        }
        #endregion

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Expanded { get; set; }

        protected string ClassName => new ClassMapper()
                 .Add("g-collapse")
                 .If("g-collapse-expanded", () => Expanded)
                 .AsString();
    }
}