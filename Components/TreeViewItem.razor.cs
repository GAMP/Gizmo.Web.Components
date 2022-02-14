using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class TreeViewItem : CustomDOMComponentBase
    {
        private bool _isExpanded;

        [CascadingParameter]
        protected TreeView Parent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public RenderFragment NestedTreeView { get; set; }

        [Parameter]
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (_isExpanded == value)
                    return;

                _isExpanded = value;

                IsExpandedChanged.InvokeAsync(_isExpanded);
            }
        }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public EventCallback<bool> IsExpandedChanged { get; set; }

        protected async Task OnClickHandler(MouseEventArgs args)
        {
            //if (IsDisabled)
            //    return;

            IsExpanded = !IsExpanded;

            if (Parent != null)
            {
                await ((TreeView)Parent).SetClickedItem(this, IsExpanded);
            }

            await OnClick.InvokeAsync(args);
        }

        internal void SetExpanded(bool expanded)
        {
            //if (IsDisabled)
                //return;

            IsExpanded = expanded;

            //StateHasChanged();
        }

        protected override void OnInitialized()
        {
            if (Parent != null)
            {
                Parent.Register(this);
                //IsDisabled = Parent.IsDisabled;
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

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-tree-view-item")
                 .If("giz-tree-view-item--expanded", () => IsExpanded)
                 .AsString();

        #endregion
    }
}