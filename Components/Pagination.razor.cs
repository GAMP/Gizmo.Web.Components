using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Pagination : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Parameter]
        public bool IsPreviousEnabled { get; set; }

        [Parameter]
        public bool IsNextEnabled { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickPrevious { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickNext { get; set; }

        #endregion

        #region EVENTS

        protected Task OnClickPreviousHandler(MouseEventArgs args)
        {
            return OnClickPrevious.InvokeAsync(args);
        }

        protected Task OnClickNextHandler(MouseEventArgs args)
        {
            return OnClickNext.InvokeAsync(args);
        }

        #endregion
    }
}