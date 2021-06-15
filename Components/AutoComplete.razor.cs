using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Gizmo.Web.Components.GizInput;

namespace Gizmo.Web.Components
{
    public partial class AutoComplete<TItemType, TValue> : InputBase<TValue>, ISelect<TValue>
    {
        #region CONSTRUCTOR
        public AutoComplete()
        {
        }
        #endregion

        #region MEMBERS
        private TValue _value;
        private string _text;

        private List _itemsList;
        private Dictionary<TValue, SelectItem<TValue>> _items = new Dictionary<TValue, SelectItem<TValue>>();
        #endregion

        #region PROPERTIES

        [Parameter]
        public ICollection<TItemType> ItemSource { get; set; }

        [Parameter]
        public Func<TItemType, TValue> ItemValueSelector { get; set; }

        [Parameter]
        public Func<TItemType, string> ItemStringSelector { get; set; }

        [Parameter]
        public RenderFragment<TItemType> ItemTemplate { get; set; }

        [Parameter]
        public TValue Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                //Find the item with this value.
                var item = ItemSource.Where(a => GetItemValue(a)?.Equals(_value) == true).FirstOrDefault();

                //If the item exists in the ItemSource.
                if (item != null)
                {
                    //Update the component's text.
                    _text = GetItemText(item);

                    StateHasChanged();
                }
                else
                {
                    //If the item does not exist then clear the value.
                    SetSelectedItem(null);
                }
            }
        }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public int MaximumHeight { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public bool OffsetY { get; set; }

        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;

        [Parameter]
        public bool HasOutline { get; set; } = true;

        [Parameter]
        public bool HasShadow { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        #endregion

        #region METHODS

        internal Task SetSelectedValue(TValue value)
        {
            if (!EqualityComparer<TValue>.Default.Equals(_value, value))
            {
                _value = value;
                return ValueChanged.InvokeAsync(_value);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        #endregion

        #region EVENTS

        protected async Task OnInputKeyDownHandler(KeyboardEventArgs args)
        {
            if (IsDisabled)
                return;

            if (args.Key == null || args.Key == "Tab")
                return;

            if (!IsOpen)
                IsOpen = true;

            //If list has items.
            //Get the index of the selected item.

            int selectedItemIndex = _itemsList.GetSelectedItemIndex();
            int listSize = _itemsList.GetListSize();

            switch (args.Key)
            {
                case "Enter":

                    if (selectedItemIndex == -1) //If not item was selected.
                    {
                        selectedItemIndex = 0; //Select the first item.
                    }
                    else
                    {
                        //Set the value of the AutoComplete based on the selected item.
                        var selectItem = _items.Where(a => a.Value.ListItem == _itemsList.SelectedItem).Select(a => a.Value).FirstOrDefault();
                        await SetSelectedItem(selectItem);
                        
                        //Close the popup.
                        IsOpen = false;

                        return;
                    }

                    break;

                case "ArrowDown":

                    if (selectedItemIndex == -1 || selectedItemIndex == listSize - 1) //If not item was selected or the last item was selected.
                    {
                        //Select the first item.
                        selectedItemIndex = 0;
                    }
                    else
                    {
                        //Select the next item.
                        selectedItemIndex += 1;
                    }

                    break;
                case "ArrowUp":

                    if (selectedItemIndex == -1 || selectedItemIndex == 0) //If not item was selected or the first item was selected.
                    {
                        //Select the last item.
                        selectedItemIndex = listSize - 1;
                    }
                    else
                    {
                        //Select the previous item.
                        selectedItemIndex -= 1;
                    }

                    break;

                default:
                    return;
            }

            //Update the selected item in the list.
            await _itemsList.SetSelectedItemIndex(selectedItemIndex);
        }

        public void OnInputHandler(ChangeEventArgs args)
        {
            _text = (string)args.Value;

            StateHasChanged();
        }

        protected Task OnInputClickHandler(MouseEventArgs args)
        {
            return Task.CompletedTask;
        }

        protected void OnMenuClickHandler(MouseEventArgs args)
        {
            IsOpen = true;
        }

        protected void OnOverlayClickHandler(MouseEventArgs args)
        {
            IsOpen = false;
        }

        #endregion

        protected override async Task OnFirstAfterRenderAsync()
        {
            //If the component initialized with a value.
            if (_value != null)
            {
                //Find the item with this value.
                var item = ItemSource.Where(a => GetItemValue(a)?.Equals(_value) == true).FirstOrDefault();

                //If the item exists in the ItemSource.
                if (item != null)
                {
                    //Update the component's text.
                    _text = GetItemText(item);

                    StateHasChanged();
                }
                else
                {
                    //If the item does not exist then clear the value.
                    await SetSelectedValue(default(TValue));
                }
            }

            await base.OnFirstAfterRenderAsync();
        }

        #region ISelect

        public void Register(SelectItem<TValue> selectItem)
        {
            _items[selectItem.Value] = selectItem;
        }

        public void Unregister(SelectItem<TValue> selectItem)
        {
            _items.Remove(selectItem.Value);
        }

        public Task SetSelectedItem(SelectItem<TValue> selectItem)
        {
            IsOpen = false;

            if (selectItem != null)
            {
                _text = selectItem.Text;
                StateHasChanged();

                return SetSelectedValue(selectItem.Value);
            }
            else
            {
                _text = string.Empty;
                StateHasChanged();

                return SetSelectedValue(default(TValue));
            }
        }

        #endregion

        protected string ClassName => new ClassMapper()
                 .Add("giz-input-select")
                 //.If("giz-select-root--disabled", () => IsDisabled)
                 //.If("giz-select-root--offset", () => OffsetY)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-input-select-dropdown-menu")
                 .Add("giz-select-dropdown-full-width")
                 .Add("giz-popup-bottom")
                 .AsString();

        private TValue GetItemValue(TItemType item)
        {
            if (ItemValueSelector != null)
            {
                return ItemValueSelector.Invoke(item);
            }

            return default(TValue);
        }

        private string GetItemText(TItemType item)
        {
            if (ItemStringSelector != null)
            {
                return ItemStringSelector.Invoke(item);
            }

            return item?.ToString();
        }

        protected IEnumerable<TItemType> GetFiltered(string text)
        {
            var result = ItemSource.Where(a => string.IsNullOrEmpty(text) || GetItemText(a)?.ToLowerInvariant().Contains(text?.ToLowerInvariant()) == true)
                        .ToList();

            return result;
        }

    }
}