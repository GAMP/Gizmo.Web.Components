using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
        public bool InitCollapsed { get; set; }

        #endregion

        //protected async Task OnKeyDownHandler(KeyboardEventArgs args)
        //{
        //    if (args.Key == null)
        //        return;

        //    if (args.Key == "Enter")
        //    {
        //        await InvokeVoidAsync("expansionPanelToggle", Ref);
        //    }
        //}

        protected async Task OnClickHeader()
        {
            await InvokeVoidAsync("expansionPanelToggle", Ref);
        }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-expansion-panel")
                 .AsString();

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (InitCollapsed)
                {
                    await InvokeVoidAsync("expansionPanelToggle", Ref);
                }
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}