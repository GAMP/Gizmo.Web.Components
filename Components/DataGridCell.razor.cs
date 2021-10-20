using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class DataGridCell<TItemType> : CustomDOMComponentBase
    {
        [Parameter]
        public DataGridColumn<TItemType> Column { get; set; }

        [Parameter]
        public TItemType Item { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
