using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Web.Components
{
    public partial class List : CustomDOMComponentBase, IDisposable
    {
        #region CONSTRUCTOR
        public List()
        {
        }
        #endregion

        private HashSet<ListItem> _items = new HashSet<ListItem>();
        private HashSet<List> _childLists = new HashSet<List>();
        private ListItem _selectedItem;

        [CascadingParameter]
        protected List ParentList { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter] public bool Disabled { get; set; }

        [Parameter]
        public ListItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;

                SetSelectedItem(value);
            }
        }
       
        internal void SetSelectedItem(ListItem item)
        {
            if (Disabled)
                return;

            if (_selectedItem == item)
                return;

            _selectedItem = item;
            _ = SelectedItemChanged.InvokeAsync(item);

            foreach (var listItem in _items.ToArray())
            {
                listItem.SetSelected(item == listItem);
            }

            foreach (var childList in _childLists.ToArray())
                childList.SetSelectedItem(item);

            ParentList?.SetSelectedItem(item);
        }

        [Parameter]
        public EventCallback<ListItem> SelectedItemChanged { get; set; }

        internal void Register(ListItem item)
        {
            _items.Add(item);
        }

        internal void Unregister(ListItem item)
        {
            _items.Remove(item);
        }

        internal void Register(List child)
        {
            _childLists.Add(child);
        }

        internal void Unregister(List child)
        {
            _childLists.Remove(child);
        }

        protected override void OnInitialized()
        {
            if (ParentList != null)
            {
                ParentList.Register(this);
                Disabled = ParentList.Disabled;
            }
        }

        public override void Dispose()
        {
            ParentList?.Unregister(this);

            base.Dispose();
        }
        protected string ClassName => new ClassMapper()
                 .Add("g-list")
                 .AsString();
    }
}