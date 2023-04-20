using Gizmo.Client.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class ExpansionPanel : CustomDOMComponentBase, IAsyncDisposable
    {
        #region CONSTRUCTOR
        public ExpansionPanel()
        {
        }
        #endregion

        private bool _isCollapsed = false;

        #region PROPERTIES

        [Parameter]
        public RenderFragment ExpansionPanelHeader { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsCollapsed
        {
            get
            {
                return _isCollapsed;
            }
            set
            {
                if (_isCollapsed == value)
                    return;

                _isCollapsed = value;
                _ = IsCollapsedChanged.InvokeAsync(_isCollapsed);
            }
        }

        [Parameter]
        public EventCallback<bool> IsCollapsedChanged { get; set; }

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
                await JsRuntime.InvokeVoidAsync("registerExpansionPanel", Ref);
                ExpansionPanelEventInterop = new ExpansionPanelEventInterop(JsRuntime);
                await ExpansionPanelEventInterop.SetupExpansionPanelEventCallback(args => ExpansionPanelHandler(args));
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private ExpansionPanelEventInterop ExpansionPanelEventInterop { get; set; }

        private Task ExpansionPanelHandler(ExpansionPanelEventArgs args)
        {
            if (args.Id == Id)
            {
                IsCollapsed = args.IsCollapsed;
            }

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            ExpansionPanelEventInterop?.Dispose();

            base.Dispose();
        }

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("unregisterExpansionPanel", Ref).ConfigureAwait(false);

            Dispose();
        }

        #endregion
    }
}
