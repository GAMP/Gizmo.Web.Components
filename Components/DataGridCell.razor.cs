using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class DataGridCell<TItemType> : CustomDOMComponentBase
    {
        [Parameter]
        public DataGridColumn<TItemType> Column { get; set; }

        [Parameter]
        public TItemType Item { get; set; }

        internal ValueTask OnDataCellMouseEvent(MouseEventArgs args, TItemType item, DataGridColumn<TItemType> column)
        {
            //called once cell data item is clicked

            return ValueTask.CompletedTask;
        }
    }
}
