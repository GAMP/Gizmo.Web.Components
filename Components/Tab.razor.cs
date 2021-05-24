using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Tab : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Tab()
        {
        }
        #endregion

        #region MEMBERS
        private int _activeItemIndex = 0;

        private List<TabItem> _items = new List<TabItem>();
        #endregion

        #region PROPERTIES
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }
        [Parameter]
        public bool IsVisible { get; set; } = true;

        [Parameter]
        public bool CanChangeTabItem { get; set; }

        public TabItem ActiveItem { get; private set; }

        [Parameter] public string TabItemClass { get; set; }

        [Parameter]
        public int ActiveItemIndex
        {
            get => _activeItemIndex;
            set
            {
                SetSelectedItem(value);
            }
        }

        [Parameter]
        public EventCallback<int> ActiveItemIndexChanged { get; set; }
        #endregion

        #region METHODS
        string GetTabItemClass(TabItem item)
        {
            var itemClassName = new ClassMapper()
             .If("gizmo-tab-active", () => item == ActiveItem)
             .If("gizmo-tab-hidden", () => !item.IsVisible)
             .If("gizmo-tab-disabled", () => item.IsDisabled).AsString();
            return itemClassName;
        }
        string ClassName()
        {
            var className = new ClassMapper()
              .Add("gizmo-tab")
             .If("gizmo-tab-disabled", () => IsDisabled)
             .If("gizmo-tab-hidden", () => !IsVisible).AsString();
            return className;
        }
        public void ActivateItem(TabItem item, bool ignoreDisabledState = false)
        {
            ActivateItem(item, null, ignoreDisabledState);
        }

        public void ActivateItem(int index, bool ignoreDisabledState = false)
        {
            var item = _items[index];
            ActivateItem(item, null, ignoreDisabledState);
        }

        private void ActivateItem(TabItem item, MouseEventArgs ev, bool ignoreDisabledState = false)
        {
            if (!item.IsDisabled || ignoreDisabledState)
            {
                ActiveItemIndex = _items.IndexOf(item);

                //Here you assume that the ActiveItem is the clicked item, but if the clicked item is disabled for some reason then the clicked item must not become the ActiveItem.
                if (ev != null)
                    ActiveItem.OnClick.InvokeAsync(ev);
                StateHasChanged();
            }
        }

        internal void Register(TabItem item)
        {
            _items.Add(item);

            //Make first tab selected default
            if (_items.Count == 1)
                ActiveItem = item;
            //Check if user set active index 2 instead of 1 then make 2nd tab active.
            if (_items.Count == ActiveItemIndex)
                ActiveItem = item;

            StateHasChanged();
        }

        internal void Unregister(TabItem item)
        {
            _items.Remove(item);
        }

        internal void SetSelectedItem(int index)
        {
            //If user set active item index 2 then keep it 2nd tab active instead of default 1st.
            if (index > 0)
                _activeItemIndex = index;

            //first time items are empty so to avoid index out of range error.
            if (_items.Count > 0)
            {
                //If the index is invalid the do nothing. (Just to be sure.)
                if (index < 0 || index >= _items.Count)
                    return;

                //If the whole tab component is disabled then do nothing.
                if (IsDisabled)
                    return;

                TabItem item = _items[index];

                //If the clicked item is already the ActiveItem then do nothing.
                if (ActiveItem == item)
                    return;

                //If the clicked item is disabled then do nothing.
                if (item.IsDisabled)
                    return;

                //Change the active item.
                _activeItemIndex = index;
                ActiveItem = item;
                _ = ActiveItemIndexChanged.InvokeAsync(_activeItemIndex);

                StateHasChanged();
            }
        }
        protected override Task OnFirstAfterRenderAsync()
        {
            //If user set active index then don't whatever active index should marked as active
            if (_activeItemIndex == 0)
            {
                var firstAvailableItem = _items.Where(a => a.IsVisible && !a.IsDisabled).FirstOrDefault();
                if (firstAvailableItem != null)
                {
                    int index = _items.IndexOf(firstAvailableItem);
                    SetSelectedItem(index);
                }
            }

            return base.OnFirstAfterRenderAsync();
        }
        #endregion
    }
}