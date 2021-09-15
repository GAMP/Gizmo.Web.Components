using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class AutoComplete<TItemType, TValue> : GizInputBase<TValue>, ISelect<TValue>
    {
        const int DEFAULT_DELAY = 500;

        #region CONSTRUCTOR
        public AutoComplete()
        {
            _deferredAction = new DeferredAction(Search);
            _delayTimeSpan = new TimeSpan(0, 0, 0, 0, _delay);
        }
        #endregion

        #region FIELDS

        private TValue _value;
        private string _text;

        private IEnumerable<TItemType> _items;
        private List _itemsList;
        private Dictionary<TValue, SelectItem<TValue>> _selectItems = new Dictionary<TValue, SelectItem<TValue>>();
        private DeferredAction _deferredAction;
        private int _delay = DEFAULT_DELAY;
        private TimeSpan _delayTimeSpan;
        #endregion

        #region PROPERTIES

        [Parameter]
        public IEnumerable<TItemType> ItemSource
        {
            get
            {
                return _items;
            }
            set
            {
                if (SearchFunction == null)
                {
                    _items = value;
                }
            }
        }

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
                TItemType item = default(TItemType);

                if (_items != null)
                {
                    item = _items.Where(a => GetItemValue(a)?.Equals(_value) == true).FirstOrDefault();
                }

                //If the item exists in the _items.
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
        public string MaximumHeight { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public bool OffsetY { get; set; }

        [Parameter]
        public InputSizes Size { get; set; } = InputSizes.Normal;

        [Parameter]
        public bool HasOutline { get; set; } = true;

        [Parameter]
        public bool HasShadow { get; set; }

        [Parameter]
        public bool IsTransparent { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public int MinimumCharacters { get; set; } = 3;

        [Parameter]
        public int Delay
        {
            get
            {
                return _delay;
            }
            set
            {
                _delay = value;

                _delayTimeSpan = new TimeSpan(0, 0, 0, 0, _delay);
            }
        }

        [Parameter]
        public Func<string, Task<IEnumerable<TItemType>>> SearchFunction { get; set; }

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
            IEnumerable<TItemType> result = null;

            if (_items != null)
            {
                result = _items.Where(a => string.IsNullOrEmpty(text) || GetItemText(a)?.ToLowerInvariant().Contains(text?.ToLowerInvariant()) == true).ToList();
            }

            return result;
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

            int activeItemIndex = _itemsList.GetSelectedItemIndex();
            int listSize = _itemsList.GetListSize();

            switch (args.Key)
            {
                case "Enter":

                    if (activeItemIndex == -1) //If not item was selected.
                    {
                        activeItemIndex = 0; //Select the first item.
                    }
                    else
                    {
                        //Set the value of the AutoComplete based on the selected item.
                        var selectItem = _selectItems.Where(a => a.Value.ListItem == _itemsList.SelectedItem).Select(a => a.Value).FirstOrDefault();
                        await SetSelectedItem(selectItem);

                        //Close the popup.
                        IsOpen = false;

                        return;
                    }

                    break;

                case "ArrowDown":

                    if (activeItemIndex == -1 || activeItemIndex == listSize - 1) //If not item was selected or the last item was selected.
                    {
                        //Select the first item.
                        activeItemIndex = 0;
                    }
                    else
                    {
                        //Select the next item.
                        activeItemIndex += 1;
                    }

                    break;
                case "ArrowUp":

                    if (activeItemIndex == -1 || activeItemIndex == 0) //If not item was selected or the first item was selected.
                    {
                        //Select the last item.
                        activeItemIndex = listSize - 1;
                    }
                    else
                    {
                        //Select the previous item.
                        activeItemIndex -= 1;
                    }

                    break;

                default:
                    return;
            }

            //Update the selected item in the list.
            await _itemsList.SetSelectedItemIndex(activeItemIndex);
        }

        public Task OnInputHandler(ChangeEventArgs args)
        {
            _text = (string)args.Value;

            if (SearchFunction != null)
            {
                if (MinimumCharacters > 0 && _text.Length >= MinimumCharacters)
                {
                    _deferredAction.Defer(_delayTimeSpan);
                }
            }

            //StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task Search()
        {
            _items = await SearchFunction(_text);

            StateHasChanged();
        }

        protected Task OnInputClickHandler(MouseEventArgs args)
        {
            return Task.CompletedTask;
        }

        protected Task OnMenuClickHandler(MouseEventArgs args)
        {
            IsOpen = true;

            return Task.CompletedTask;
        }

        protected Task OnClickOverlayHandler(MouseEventArgs args)
        {
            IsOpen = false;

            return Task.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnFirstAfterRenderAsync()
        {
            //If the component initialized with a value.
            if (_value != null)
            {
                //Find the item with this value.
                TItemType item = default(TItemType);

                if (_items != null)
                {
                    item = _items.Where(a => GetItemValue(a)?.Equals(_value) == true).FirstOrDefault();
                }

                //If the item exists in the _items.
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

        #endregion

        #region ISelect

        public void Register(SelectItem<TValue> selectItem)
        {
            _selectItems[selectItem.Value] = selectItem;
        }

        public void Unregister(SelectItem<TValue> selectItem)
        {
            _selectItems.Remove(selectItem.Value);
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

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-input-autocomplete")
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-input-autocomplete-dropdown-menu")
                 .Add("giz-autocomplete-dropdown-full-width")
                 .AsString();

        #endregion

    }
}