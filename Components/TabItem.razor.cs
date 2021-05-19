using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;

namespace Gizmo.Web.Components
{
    public partial class TabItem : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public TabItem()
        {
        }
        #endregion

        #region MEMBERS

        private bool _isSelected;

        #endregion

        [CascadingParameter]
        protected Tab Parent { get; set; }

        #region PROPERTIES

        [Parameter]
        public RenderFragment Header { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        #endregion

        protected string ClassName => new ClassMapper()
                 .If("gizmo-tab-content-active", () => _isSelected)
                 .AsString();

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

        internal void SetSelected(bool selected)
        {
            if (_isSelected == selected)
                return;

            _isSelected = selected;

            StateHasChanged();
        }

    }
}