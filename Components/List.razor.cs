using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Web.Components
{

    public partial class List : CustomDOMComponentBase
    {
        public enum ListDirection
        {
            left,
            right
        }

        #region CONSTRUCTOR
        public List()
        {
        }
        #endregion

        #region MEMBERS

        private HashSet<ListItem> _items = new HashSet<ListItem>();
        private HashSet<List> _childLists = new HashSet<List>();
        private ListItem _selectedItem;

        #endregion

        [CascadingParameter]
        protected List ParentList { get; set; }


        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

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

        [Parameter]
        public EventCallback<ListItem> SelectedItemChanged { get; set; }

        [Parameter()]
        public ListDirection Direction { get; set; } = ListDirection.right;

        #endregion

        internal void SetSelectedItem(ListItem item)
        {
            if (IsDisabled)
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
                IsDisabled = ParentList.IsDisabled;
            }
        }

        public override void Dispose()
        {
            ParentList?.Unregister(this);

            base.Dispose();
        }
        
        protected string ClassName => new ClassMapper()
                 .Add("giz-list")
                 .Add($"giz-list--{Direction}")
                 .AsString();
    }
}