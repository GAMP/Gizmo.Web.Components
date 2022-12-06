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
        #region FIELDS

        private bool _isSelected = false;
        private bool _isExpanded = false;
        private TreeView _treeView;

        #endregion

        #region PROPERTIES

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
                _ = IsExpandedChanged.InvokeAsync(_isExpanded);
            }
        }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnDoubleClick { get; set; }

        [Parameter]
        public EventCallback<bool> IsExpandedChanged { get; set; }

        #endregion

        #region EVENTS

        protected async Task OnClickExpandButtonHandler(MouseEventArgs args)
        {
            //if (IsDisabled)
            //    return;

            IsExpanded = !IsExpanded;

            //If collapsed and the selected item is under this item then set this item as clicked item.
            if (!IsExpanded && IsChildSelected())
            {
                if (Parent != null)
                {
                    await Parent.SetClickedItem(this);
                }

                await OnClick.InvokeAsync(args);
            }
        }

        protected async Task OnClickHandler(MouseEventArgs args)
        {
            //if (IsDisabled)
            //    return;

            if (Parent != null)
            {
                await Parent.SetClickedItem(this);
            }

            await OnClick.InvokeAsync(args);
        }

        protected async Task OnDoubleClickEvent(MouseEventArgs args)
        {
            //if (IsDisabled)
            //    return;

            IsExpanded = !IsExpanded;

            if (Parent != null)
            {
                await Parent.SetDoubleClickedItem(this);
            }

            await OnDoubleClick.InvokeAsync(args);
        }

        protected async Task ContextMenuHandler(MouseEventArgs args)
        {
            if (Parent != null)
            {
                if (args.Button == 2)
                {
                    await Parent.SetClickedItem(this);
                    await Parent.OpenContextMenu(args.ClientX, args.ClientY);
                }
            }
        }

        #endregion

        #region METHODS

        internal void SetSelected(bool value)
        {
            //if (IsDisabled)
            //    return;

            if (_isSelected == value)
                return;

            _isSelected = value;

            StateHasChanged();
        }

        internal void SetExpanded(bool expanded)
        {
            //if (IsDisabled)
            //return;

            IsExpanded = expanded;

            //StateHasChanged();
        }

        internal bool IsChildSelected()
        {
            if (_isSelected)
                return true;

            if (NestedTreeView != null)
            {
                return _treeView.IsChildSelected();
            }

            return false;
        }

        #endregion

        #region OVERRIDES

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

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-tree-view-item")
                 .If("giz-tree-view-item--expanded", () => IsExpanded)
                 .If("selected", () => _isSelected)
                 .AsString();

        #endregion
    }
}