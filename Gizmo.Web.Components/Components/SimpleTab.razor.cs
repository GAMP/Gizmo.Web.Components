using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class SimpleTab : CustomDOMComponentBase
    {
        #region FIELDS

        private SimpleTabHeader _tabHeader;
        private int _selectedIndex = 0;
        private List<SimpleTabItem> _items = new List<SimpleTabItem>();
        private SimpleTabItem _previousSelectedItem;

        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool IsVisible { get; set; } = true;

        [Parameter]
        public bool RenderOnlyActive { get; set; } = true;

        [Parameter]
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (_selectedIndex == value)
                    return;

                _selectedIndex = value;
                UpdateSelected();
                _ = SelectedIndexChanged.InvokeAsync(_selectedIndex);
            }
        }

        [Parameter]
        public EventCallback<int> SelectedIndexChanged { get; set; }

        #endregion

        #region METHODS

        internal void Register(SimpleTabItem item)
        {
            _items.Add(item);
        }

        internal void Unregister(SimpleTabItem item)
        {
            _items.Remove(item);
        }

        internal void SetSelectedIndex(int index)
        {
            SelectedIndex = index;
        }

        private void UpdateSelected()
        {
            if (_items.Count > 0)
            {
                //If the index is invalid the do nothing. (Just to be sure.)
                if (_selectedIndex < 0 || _selectedIndex >= _items.Count)
                    return;

                //If the whole tab component is disabled then do nothing.
                if (IsDisabled)
                    return;

                _tabHeader?.SetSelectedIndex(_selectedIndex);

                if (_previousSelectedItem != null)
                {
                    _previousSelectedItem.SetSelected(false);
                }

                _previousSelectedItem = _items[_selectedIndex];
                _previousSelectedItem.SetSelected(true);
            }
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .Add("giz-simple-tab")
                .If("giz-simple-tab--hidden", () => !IsVisible)
                .If("disabled", () => IsDisabled)
                .AsString();

        #endregion

        protected override Task OnFirstAfterRenderAsync()
        {
            //Validate selection
            if (_selectedIndex < 0 || _selectedIndex >= _items.Count ||
                !_items[_selectedIndex].IsVisible || _items[_selectedIndex].IsDisabled)
            {
                var firstAvailableItem = _items.Where(a => a.IsVisible && !a.IsDisabled).FirstOrDefault();
                if (firstAvailableItem != null)
                {
                    int index = _items.IndexOf(firstAvailableItem);
                    SetSelectedIndex(index);
                }
            }
            else
            {
                UpdateSelected();
            }

            StateHasChanged();

            return base.OnFirstAfterRenderAsync();
        }
    }
}