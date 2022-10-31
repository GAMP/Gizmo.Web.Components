using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class FileInput : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback<InputFileChangeEventArgs> OnChange { get; set; }

        #endregion

        private Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            return OnChange.InvokeAsync(e);
        }
    }
}