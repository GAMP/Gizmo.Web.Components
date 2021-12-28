﻿using Gizmo.Web.Components.Extensions;
using Gizmo.Web.Components.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Select<TValue> : GizInputBase<TValue>, ISelect<TValue>
    {
        #region CONSTRUCTOR
        public Select()
        {
        }
        #endregion

        #region FIELDS

        private Dictionary<TValue, SelectItem<TValue>> _items = new Dictionary<TValue, SelectItem<TValue>>();
        private TValue _value;
        private SelectItem<TValue> _selectedItem;
        private List _popupContent;
        private bool _isOpen;
        private double _popupX;
        private double _popupY;
        private double _popupWidth;

        private bool _hasParsingErrors;
        private string _parsingErrors;

        #endregion

        #region PROPERTIES

        [Parameter]
        public ValidationErrorStyles ValidationErrorStyle { get; set; } = ValidationErrorStyles.Label;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public TValue Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (EqualityComparer<TValue>.Default.Equals(_value, value))
                    return;

                _value = value;

                if (_value != null)
                {
                    if (_items.ContainsKey(_value))
                    {
                        SelectItem(false, string.Empty, _items[_value]);
                    }
                    else
                    {
                        SelectItem(true, "The field is required.", null);
                    }
                }
                else
                {
                    SelectItem(false, string.Empty, null);
                }
            }
        }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string MaximumHeight { get; set; }

        [Parameter]
        public PopupOpenDirections OpenDirection { get; set; } = PopupOpenDirections.Bottom;

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
        public string Width { get; set; } = "20rem";

        [Parameter]
        public string Placeholder { get; set; }

        public bool IsValid => !_hasParsingErrors && _isValid;

        public string ValidationMessage => _hasParsingErrors ? _parsingErrors : _validationMessage;

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

        protected async Task OnClickInput()
        {
            if (!IsDisabled)
            {
                if (!_isOpen)
                    await Open();
                else
                    _isOpen = false;
            }
        }

        protected async Task OnInputKeyDownHandler(KeyboardEventArgs args)
        {
            if (IsDisabled)
                return;

            if (args.Key == null || args.Key == "Tab")
                return;

            if (!_isOpen)
                await Open();

            //If list has items.
            //Get the index of the selected item.

            int activeItemIndex = _popupContent.GetActiveItemIndex();
            int listSize = _popupContent.GetListSize();

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
                        var selectItem = _items.Where(a => a.Value.ListItem == _popupContent.ActiveItem).Select(a => a.Value).FirstOrDefault();
                        await SetSelectedItem(selectItem);
                        await _popupContent.SetActiveItemIndex(activeItemIndex);

                        //Close the popup.
                        _isOpen = false;

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
            await _popupContent.SetActiveItemIndex(activeItemIndex);
        }

        #endregion

        #region OVERRIDES

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (_value != null)
            {
                if (_items.ContainsKey(_value))
                {
                    SelectItem(false, string.Empty, _items[_value]);
                }
                else
                {
                    SelectItem(true, "The field is required.", null);
                }
            }
            else
            {
                SelectItem(false, string.Empty, null);
            }

            return base.OnAfterRenderAsync(firstRender);
        }

        public override void Validate(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            validationMessageStore.Clear();

            if (_hasParsingErrors)
            {
                validationMessageStore.Add(fieldIdentifier, _parsingErrors);
            }
        }

        #endregion

        #region METHODS

        public void Register(SelectItem<TValue> selectItem, TValue value)
        {
            if (value == null)
                return;

            _items[value] = selectItem;
        }

        public void Update(SelectItem<TValue> selectItem, TValue value)
        {
            if (value == null)
                return;

            var actualItem = _items.Where(a => a.Value == selectItem).FirstOrDefault();
            if (!actualItem.Equals(default(KeyValuePair<TValue, SelectItem<TValue>>)) && actualItem.Key != null)
            {
                _items.Remove(actualItem.Key);
            }

            _items[value] = selectItem;
        }

        public void Unregister(SelectItem<TValue> selectItem, TValue value)
        {
            if (value == null)
                return;

            var actualItem = _items.Where(a => a.Value == selectItem).FirstOrDefault();
            if (!actualItem.Equals(default(KeyValuePair<TValue, SelectItem<TValue>>)))
            {
                _items.Remove(actualItem.Key);
            }
        }

        public void SelectItem(bool hasParsingErrors, string parsingErrors, SelectItem<TValue> selectItem)
        {
            bool refresh = false;

            if (_hasParsingErrors != hasParsingErrors)
            {
                _hasParsingErrors = hasParsingErrors;
                _parsingErrors = parsingErrors;

                refresh = true;
            }

            if (_selectedItem != selectItem)
            {
                _selectedItem = selectItem;

                refresh = true;
            }

            if (refresh)
            {
                StateHasChanged();
            }
        }

        public Task SetSelectedItem(SelectItem<TValue> selectItem)
        {
            _isOpen = false;

            if (_selectedItem == selectItem)
                return Task.CompletedTask;

            _selectedItem = selectItem;

            _hasParsingErrors = false;
            _parsingErrors = String.Empty;

            StateHasChanged();

            if (selectItem != null)
                return SetSelectedValue(selectItem.Value);
            else
                return SetSelectedValue(default(TValue));
        }

        private async Task Open()
        {
            if (OpenDirection == PopupOpenDirections.Cursor)
            {
                var windowSize = await JsInvokeAsync<WindowSize>("getWindowSize");
                var popupContentSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", _popupContent.Ref);

                var inputSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", Ref);

                _popupX = inputSize.Left;
                _popupWidth = inputSize.Width;

                if (inputSize.Bottom + popupContentSize.Height > windowSize.Height)
                {
                    _popupY = windowSize.Height - popupContentSize.Height;
                }
                else
                {
                    _popupY = inputSize.Bottom;
                }
            }

            int activeItemIndex = _popupContent.GetSelectedItemIndex();
            await _popupContent.SetActiveItemIndex(activeItemIndex);

            _isOpen = true;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-input-select")
                 .If("giz-input-select--full-width", () => IsFullWidth)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-input-select__dropdown")
                 .If("giz-input-select__dropdown--cursor", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If("giz-input-select__dropdown--full-width", () => OpenDirection != PopupOpenDirections.Cursor)
                 .AsString();

        protected string PopupStyleValue => new StyleMapper()
                 .If($"top: {_popupY.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If($"left: {_popupX.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If($"width: {_popupWidth.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .AsString();

        #endregion

    }
}