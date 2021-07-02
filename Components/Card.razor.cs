using Gizmo.Web.Components.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class Card : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Card()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment CardHeader { get; set; }

        [Parameter]
        public RenderFragment CardBody { get; set; }

        [Parameter]
        public RenderFragment CardFooter { get; set; }

        [Parameter]
        public string BackgroundColor { get; set; }

        [Parameter]
        public bool HasShadow { get; set; }

        [Parameter]
        public int CornerRadius { get; set; } = 4;

        [Parameter]
        public int MaximumWidth { get; set; }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-card")
                 .If($"g-shadow-8", () => HasShadow)
                 .AsString();

        protected string StyleValue => new StyleMapper()
                 .Add($"background-color: {BackgroundColor}")
                 .Add($"border-radius: {CornerRadius}px")
                 .If($"max-width: {MaximumWidth}px", () => MaximumWidth > 0)
                 .AsString();

        #endregion

    }
}