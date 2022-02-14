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
        private List<TreeViewItem> _items = new List<TreeViewItem>();

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public EventCallback<TreeViewItem> OnClickItem { get; set; }

        internal void Register(TreeViewItem item)
        {
            _items.Add(item);
        }

        internal void Unregister(TreeViewItem item)
        {
            _items.Remove(item);
        }

        internal async Task SetClickedItem(TreeViewItem treeViewItem, bool isExpanded)
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
