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

        private List<HierarchicalTreeViewItem<TItemType>> _items = new List<HierarchicalTreeViewItem<TItemType>>();

        #endregion

        #region PROPERTIES

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

        #endregion

        #region METHODS

        internal void Register(HierarchicalTreeViewItem<TItemType> item)
        {
            _items.Add(item);
        }

        internal void Unregister(HierarchicalTreeViewItem<TItemType> item)
        {
            _items.Remove(item);
        }

        #endregion

        internal async Task SetClickedItem(HierarchicalTreeViewItem<TItemType> treeViewItem, bool isExpanded)
        {
            await OnClickItem.InvokeAsync(treeViewItem);
        }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-tree-view")
                 .AsString();

        #endregion
    }
}
