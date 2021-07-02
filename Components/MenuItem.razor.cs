using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class MenuItem : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public MenuItem()
        {
        }
        #endregion

        #region PROPERTIES

        internal ListItem ListItem { get; set; }

        [CascadingParameter]
        protected Menu Parent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public string Href { get; set; }

        [Parameter]
        public RenderFragment NestedList { get; set; }

        [Parameter]
        public bool IsExpanded { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        #endregion

        #region EVENTS

        protected Task OnListItemClickHandler(MouseEventArgs args)
        {
            Parent.Close();

            return Task.CompletedTask;
        }

        #endregion
    }
}