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

        #region PROPERTIES
        [CascadingParameter]
        protected Tab Parent { get; set; }
        [Parameter]
        public RenderFragment Header { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }
        [Parameter]
        public bool IsVisible { get; set; } = true;

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        #endregion

        protected string ClassName => new ClassMapper()
                .Add("gizmo-tab-content-active")
                .AsString();
        #region METHODS
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
    }
}