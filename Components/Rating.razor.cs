using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class Rating
    {
        [Parameter]
        public int Value { get; set; }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-rating")
                 .AsString();

        #endregion
    }
}