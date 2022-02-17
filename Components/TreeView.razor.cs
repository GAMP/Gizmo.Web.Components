using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class TreeView : CustomDOMComponentBase
    {
        #region FIELDS

        private List<TreeViewItem> _items = new List<TreeViewItem>();
        private List<TreeView> _childTreeViews = new List<TreeView>();

        private double _clientX;
        private double _clientY;
        private Menu _contextMenu;

        #endregion

        #region PROPERTIES

        [CascadingParameter]
        protected TreeView ParentTreeView { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public EventCallback<TreeViewItem> OnClickItem { get; set; }

        [Parameter]
        public EventCallback<TreeViewItem> OnDoubleClickItem { get; set; }

        [Parameter]
        public RenderFragment ContextMenu { get; set; }

        #endregion

        #region METHODS

        internal void Register(TreeViewItem item)
        {
            _items.Add(item);
        }

        internal void Unregister(TreeViewItem item)
        {
            _items.Remove(item);
        }

        internal void Register(TreeView child)
        {
            _childTreeViews.Add(child);
        }

        internal void Unregister(TreeView child)
        {
            _childTreeViews.Remove(child);
        }

        internal async Task SetClickedItem(TreeViewItem treeViewItem, bool findRoot = true)
        {
            if (findRoot && ParentTreeView != null)
            {
                await ParentTreeView.SetClickedItem(treeViewItem, true);
            }
            else
            {
                foreach (var item in _items.ToArray())
                {
                    item.SetSelected(item == treeViewItem);
                }

                foreach (var childTreeView in _childTreeViews.ToArray())
                {
                    await childTreeView.SetClickedItem(treeViewItem, false);
                }

                if (findRoot)
                {
                    //Raise event only from root.
                    await OnClickItem.InvokeAsync(treeViewItem);
                }
            }
        }

        internal async Task SetDoubleClickedItem(TreeViewItem treeViewItem, bool findRoot = true)
        {
            if (findRoot && ParentTreeView != null)
            {
                await ParentTreeView.SetDoubleClickedItem(treeViewItem, true);
            }
            else
            {
                if (findRoot)
                {
                    //Raise event only from root.
                    await OnDoubleClickItem.InvokeAsync(treeViewItem);
                }
            }
        }

        internal async Task OpenContextMenu(double clientX, double clientY)
        {
            if (ParentTreeView != null)
            {
                await ParentTreeView.OpenContextMenu(clientX, clientY);
            }
            else
            {
                var windowSize = await JsInvokeAsync<WindowSize>("getWindowSize");
                var contextMenuSize = await _contextMenu.GetListBoundingClientRect();

                if (clientX > windowSize.Width / 2)
                {
                    //Open direction right to left.
                    _clientX = clientX - contextMenuSize.Width;
                    _contextMenu.SetDirection(ListDirections.Left);
                }
                else
                {
                    _clientX = clientX;
                    _contextMenu.SetDirection(ListDirections.Right);
                }

                if (clientY > windowSize.Height / 2)
                {
                    //Open direction bottom to top.
                    _clientY = clientY - contextMenuSize.Height;
                    _contextMenu.ExpandBottomToTop = true;
                }
                else
                {
                    _clientY = clientY;
                    _contextMenu.ExpandBottomToTop = false;
                }

                _contextMenu.Open(_clientX, _clientY);
            }
        }

        internal bool IsChildSelected()
        {
            foreach (var item in _items)
            {
                if (item.IsChildSelected())
                    return true;
            }

            return false;
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            if (ParentTreeView != null)
            {
                ParentTreeView.Register(this);
                IsDisabled = ParentTreeView.IsDisabled;
            }
        }

        public override void Dispose()
        {
            try
            {
                if (ParentTreeView != null)
                {
                    ParentTreeView.Unregister(this);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-tree-view")
                 .AsString();

        #endregion
    }
}