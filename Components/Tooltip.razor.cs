using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class Tooltip : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Tooltip()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public RenderFragment TooltipContent { get; set; }

        [Parameter]
        public string Text { get; set; }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-tooltip-root")
                 .AsString();

        protected string TooltipClassName => new ClassMapper()
                 .Add("giz-tooltip")
                 .AsString();

        #endregion

    }
}