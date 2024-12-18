using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class SimpleTabItem : CustomDOMComponentBase
    {
        private bool _isSelected;

        private bool _shouldRender;

        [CascadingParameter]
        protected SimpleTab Parent { get; set; }

        #region PROPERTIES

        [Parameter]
        public RenderFragment SimpleTabItemHeader { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool IsVisible { get; set; } = true;

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        #endregion

        #region METHODS

        internal void SetSelected(bool selected)
        {
            if (_isSelected == selected)
                return;

            _isSelected = selected;
            _shouldRender = true;

            StateHasChanged();
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            if (Parent != null)
            {
                Parent.Register(this);
            }
        }

        public override void Dispose()
        {
            try
            {
                if (Parent != null)
                {
                    Parent.Unregister(this);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .Add("giz-simple-tab-item")
                .If("active", () => _isSelected)
                .AsString();

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                _shouldRender = false;
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }
        
        protected override bool ShouldRender()
        {
            return _shouldRender;
        }
    }
}
