using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class Rating
    {
        [Parameter]
        public decimal Value { get; set; }

        [Parameter]
        public string BackgroundColor { get; set; } = "#585d6f";

        [Parameter]
        public string Color { get; set; } = "#ffffff";

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-rating")
                 .AsString();

        #endregion
    }
}