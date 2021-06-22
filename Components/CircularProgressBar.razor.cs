using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class CircularProgressBar
    {
        [Parameter]
        public decimal Value { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string ClassName => new ClassMapper()
                 .Add("giz-circular-progressbar")
                 .AsString();

    }
}
