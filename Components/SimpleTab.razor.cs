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
        private int _selectedItemIndex = 0;
        private SimpleTabItem _selectedItem;
        private List<SimpleTabItem> _items = new List<SimpleTabItem>();

        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool IsVisible { get; set; } = true;

        [Parameter]
        public int SelectedItemIndex
        {
            get => _selectedItemIndex;
            set
            {
                SetSelectedIndex(value);
            }
        }

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

        internal void SetSelectedItem(SimpleTabItem item)
        {
            int index = _items.IndexOf(item);
            SetSelectedIndex(index);
        }

        internal void SetSelectedIndex(int index)
        {
            if (_items.Count > 0)
            {
                //If the index is invalid the do nothing. (Just to be sure.)
                if (index < 0 || index >= _items.Count)
                    return;

                //If the whole tab component is disabled then do nothing.
                if (IsDisabled)
                    return;

                SimpleTabItem item = _items[index];

                //If the clicked item is disabled then do nothing.
                if (item.IsDisabled)
                    return;

                //If the clicked item is already the ActiveItem then do nothing.
                if (_selectedItem == item)
                    return;

                //Change the active item.
                var previousSelectedItem = _selectedItem;
                _selectedItemIndex = index;
                _selectedItem = item;

                _tabHeader?.SetSelectedItem(_selectedItem);
                if (previousSelectedItem != null)
                    previousSelectedItem.SetSelected(false);
                _selectedItem.SetSelected(true);
            }
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .Add("giz-simple-tab")
                .If("giz-simple-tab--disabled", () => IsDisabled)
                .If("giz-simple-tab--hidden", () => !IsVisible)
                .AsString();

        #endregion

        protected override Task OnFirstAfterRenderAsync()
        {
            //If user set active index then don't whatever active index should marked as active
            if (_selectedItemIndex == 0)
            {
                var firstAvailableItem = _items.Where(a => a.IsVisible && !a.IsDisabled).FirstOrDefault();
                if (firstAvailableItem != null)
                {
                    int index = _items.IndexOf(firstAvailableItem);
                    SetSelectedIndex(index);
                }
            }

            return base.OnFirstAfterRenderAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

    }
}