using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class HierarchicalTreeView<TItemType> : CustomDOMComponentBase
    {
        #region FIELDS

        private Dictionary<TItemType, HierarchicalTreeViewItem<TItemType>> _items = new Dictionary<TItemType, HierarchicalTreeViewItem<TItemType>>();
        private List<HierarchicalTreeView<TItemType>> _childTreeViews = new List<HierarchicalTreeView<TItemType>>();

        private double _clientX;
        private double _clientY;
        private Menu _contextMenu;

        #endregion

        #region PROPERTIES

        [CascadingParameter]
        protected HierarchicalTreeView<TItemType> ParentTreeView { get; set; }

        [Parameter]
        public RenderFragment<TItemType> DataTemplate { get; set; }

        [Parameter]
        public ICollection<TItemType> ItemSource { get; set; }

        [Parameter]
        public string HierarchicalItemSource { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public EventCallback<HierarchicalTreeViewItem<TItemType>> OnClickItem { get; set; }

        [Parameter]
        public EventCallback<HierarchicalTreeViewItem<TItemType>> OnDoubleClickItem { get; set; }

        [Parameter]
        public RenderFragment ContextMenu { get; set; }

        #endregion

        #region METHODS

        internal void Register(HierarchicalTreeViewItem<TItemType> treeViewItem, TItemType item)
        {
            if (item == null)
                return;

            _items[item] = treeViewItem;
        }

        internal void UpdateItem(HierarchicalTreeViewItem<TItemType> treeViewItem, TItemType item)
        {
            if (item == null)
                return;

            var actualItem = _items.Where(a => a.Value == treeViewItem).FirstOrDefault();
            if (!actualItem.Equals(default(KeyValuePair<TItemType, HierarchicalTreeViewItem<TItemType>>)) && actualItem.Key != null)
            {
                _items.Remove(actualItem.Key);
            }

            _items[item] = treeViewItem;
        }

        internal void Unregister(HierarchicalTreeViewItem<TItemType> treeViewItem, TItemType item)
        {
            if (item == null)
                return;

            var actualItem = _items.Where(a => a.Value == treeViewItem).FirstOrDefault();
            if (!actualItem.Equals(default(KeyValuePair<TItemType, HierarchicalTreeViewItem<TItemType>>)))
            {
                _items.Remove(actualItem.Key);
            }
        }

        internal void Register(HierarchicalTreeView<TItemType> child)
        {
            _childTreeViews.Add(child);
        }

        internal void Unregister(HierarchicalTreeView<TItemType> child)
        {
            _childTreeViews.Remove(child);
        }

        internal async Task SetClickedItem(HierarchicalTreeViewItem<TItemType> treeViewItem, bool findRoot = true)
        {
            if (findRoot && ParentTreeView != null)
            {
                await ParentTreeView.SetClickedItem(treeViewItem, true);
            }
            else
            {
                treeViewItem.SetSelected(true);

                foreach (var item in _items.Where(a => a.Value != treeViewItem).ToArray().ToArray())
                {
                    item.Value.SetSelected(false);
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

        internal async Task SetDoubleClickedItem(HierarchicalTreeViewItem<TItemType> treeViewItem, bool findRoot = true)
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
                if (item.Value.IsChildSelected())
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
                 .Add("giz-hierarchical-tree-view")
                 .AsString();

        #endregion

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (!firstRender)
        //    {
        //        await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
        //    }

        //    await base.OnAfterRenderAsync(firstRender);
        //}
    }
}
