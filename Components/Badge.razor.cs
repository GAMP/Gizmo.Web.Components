using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class Badge : CustomDOMComponentBase
    {
        public enum BadgeSize
        {
            Small = 0,
            Normal = 1
        }

        #region CONSTRUCTOR
        public Badge()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public bool IsInline { get; set; }

        [Parameter]
        public BadgeSize Size { get; set; } = BadgeSize.Normal;

        [Parameter]
        public string Color { get; set; }

        [Parameter]
        public string BackgroundColor { get; set; } = "#5a67f2";

        #endregion

        protected string ClassName => new ClassMapper()
                 .Add("giz-badge")
                 .If("corner", () => !IsInline)
                 .If("small", () => Size == BadgeSize.Small)
                 .AsString();
    }
}