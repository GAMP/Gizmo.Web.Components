using Gizmo.Web.Components.Extensions;
using Gizmo.Web.Components.Infrastructure;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class List : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public List()
        {
        }
        #endregion

        #region FIELDS

        private List<ListItem> _items = new List<ListItem>();
        private HashSet<List> _childLists = new HashSet<List>();
        private ListItem _selectedItem;

        #endregion

        #region PROPERTIES

        [CascadingParameter]
        protected List ParentList { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool CanClick { get; set; }

        [Parameter]
        public bool CanSelect { get; set; }

        [Parameter]
        public bool PreserveIconSpace { get; set; }

        [Parameter]
        public ListItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;

                _ = SetSelectedItem(value);
            }
        }

        [Parameter]
        public EventCallback<ListItem> SelectedItemChanged { get; set; }

        [Parameter]
        public EventCallback<ListItem> OnClickItem { get; set; }

        [Parameter()]
        public ListDirections Direction { get; set; } = ListDirections.Right;

        [Parameter]
        public string MaximumHeight { get; set; }

        #endregion

        #region METHODS

        internal async Task SetSelectedItem(ListItem item)
        {
            if (IsDisabled)
                return;

            if (_selectedItem == item)
                return;

            _selectedItem = item;
            await SelectedItemChanged.InvokeAsync(item);

            foreach (var listItem in _items.ToArray())
                listItem.SetSelected(item == listItem);

            foreach (var childList in _childLists.ToArray())
                await childList.SetSelectedItem(item);

            if (ParentList != null)
                await ParentList.SetSelectedItem(item);
        }

        internal async Task SetClickedItem(ListItem item)
        {
            if (ParentList != null)
                await ParentList.SetClickedItem(item);

            await OnClickItem.InvokeAsync(item);

            await SetSelectedItem(item);
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

        internal int GetSelectedItemIndex()
        {
            if (_selectedItem != null)
                return _items.IndexOf(_selectedItem);
            else
                return -1;
        }

        internal async Task SetSelectedItemIndex(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                var item = _items[index];
                await SetSelectedItem(item);
                await InvokeVoidAsync("scrollListItemIntoView", item.Ref);
            }
        }

        internal int GetListSize()
        {
            return _items.Count;
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            if (ParentList != null)
            {
                ParentList.Register(this);
                IsDisabled = ParentList.IsDisabled;
                Direction = ParentList.Direction;
            }
        }

        public override void Dispose()
        {
            ParentList?.Unregister(this);

            base.Dispose();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-list")
                 .Add($"giz-list--{Direction.ToDescriptionString()}")
                 .If("giz-list--clickable", () => CanClick)
                 .If("giz-list--selectable", () => CanSelect)
                 .AsString();

        protected string StyleValue => new StyleMapper()
                 .If($"max-height: {MaximumHeight}", () => !string.IsNullOrEmpty(MaximumHeight))
                 .AsString();

        #endregion
    }
}