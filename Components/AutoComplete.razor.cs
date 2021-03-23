using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class AutoComplete : ComponentBase
    {
        public string Prop { get { return "Hello"; } }
        private int currentCount = 0;

        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
