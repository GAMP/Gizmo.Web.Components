using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class ExpansionPanel : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public ExpansionPanel()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment Header { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Expanded { get; set; }

        #endregion

        protected async Task OnClickHeader()
        {
            Expanded = !Expanded;

            await InvokeVoidAsync("expansionPanelToggle", Ref);
        }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-expansion-panel")
                 //.If("giz-expansion-panel--expanded", () => Expanded)
                 .AsString();

        #endregion

    }
}