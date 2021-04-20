using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class DataGridDetailTemplate : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public DataGridDetailTemplate()
        {
        }
        #endregion

        [Parameter]
        public int Columns { get; set; }

        [Parameter()]
        public RenderFragment ChildContent { get; set; }
    }
}
