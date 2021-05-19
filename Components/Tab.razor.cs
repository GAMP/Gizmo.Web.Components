using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Web.Components
{
    public partial class Tab : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Tab()
        {
        }
        #endregion

        private int _activeItemIndex = 0;
        private List<TabItem> _items = new List<TabItem>();

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool CanChangeTabItem { get; set; }

        //[Parameter]
        //public TabItem SelectedItem { get; set; }
        public TabItem ActiveItem { get; private set; }

        //[Parameter]
        //public EventCallback<TabItem> SelectedItemChanged { get; set; }

        [Parameter] public string TabItemClass { get; set; }

        string GetTabItemClass(TabItem item)
        {
            //item.Selected = true;
            var itemClassName = new ClassMapper()
             .If("gizmo-tab-active", () => item == ActiveItem)
             .If("gizmo-tab-disabled", () => Disabled).AsString();
            return itemClassName;
        }

        //You don't need this anymore.
        //string GetTabItemContentClass(TabItem item)
        //{
        //    var itemContentClassName = new ClassMapper()
        //     .If("gizmo-tab-content-active", () => item == ActiveItem).AsString();
        //    return itemContentClassName;
        //}

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

        public void ActivateItem(TabItem item, bool ignoreDisabledState = false)
        {
            ActivateItem(item, null, ignoreDisabledState);
        }

        public void ActivateItem(int index, bool ignoreDisabledState = false)
        {
            var item = _items[index];
            ActivateItem(item, null, ignoreDisabledState);
        }

        //public void ActivateItem(object id, bool ignoreDisabledState = false)
        //{
        //    var item = _items.Where((p) => p.ID == id).FirstOrDefault();
        //    if (item != null)
        //        ActivateItem(item, null, ignoreDisabledState);
        //}

        private void ActivateItem(TabItem item, MouseEventArgs ev, bool ignoreDisabledState = false)
        {
            if (!item.Disabled || ignoreDisabledState)
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
            StateHasChanged();
        }

        internal void Unregister(TabItem item)
        {
            _items.Remove(item);
        }

        internal void SetSelectedItem(int index)
        {
            //If the index is invalid the do nothing. (Just to be sure.)
            if (index < 0 || index >= _items.Count)
                return;

            //If the whole tab component is disabled then do nothing.
            if (Disabled)
                return;

            TabItem item = _items[index];

            //If the clicked item is already the ActiveItem then do nothing.
            if (ActiveItem == item)
                return;

            //If the clicked item is disabled then do nothing.
            if (item.Disabled)
                return;

            //Change the active item.
            _activeItemIndex = index;
            ActiveItem = item;
            _ = ActiveItemIndexChanged.InvokeAsync(_activeItemIndex);

            //Change the selected flag in all items.
            foreach (var tabItem in _items.ToArray())
            {
                tabItem.SetSelected(item == tabItem);
            }
        }
    }
}