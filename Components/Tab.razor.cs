using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace Gizmo.Web.Components
{
    public partial class Tab : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Tab()
        {
        }
        #endregion

        private HashSet<TabItem> _items = new HashSet<TabItem>();

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool CanChangeTabItem { get; set; }

        [Parameter]
        public TabItem SelectedItem { get; set; }

        [Parameter]
        public EventCallback<TabItem> SelectedItemChanged { get; set; }

        internal void Register(TabItem item)
        {
            _items.Add(item);
        }

        internal void Unregister(TabItem item)
        {
            _items.Remove(item);
        }
    }
}