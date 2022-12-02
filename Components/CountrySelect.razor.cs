using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class CountrySelect<TValue> : GizInputBase<TValue>, IGizInput
    {
        #region CONSTRUCTOR
        public CountrySelect()
        {
        }
        #endregion

        #region FIELDS

        private Country _selectedCountry;
        private List _popupContent;
        private ElementReference _inputElement;
        private bool _isOpen;
        private double _popupX;
        private double _popupY;
        private double _popupWidth;

        private bool _hasParsingErrors;
        private string _parsingErrors;
        private ValidationMessageStore _validationMessageStore;

        private bool _clickHandled = false;

        #endregion

        #region PROPERTIES

        #region IGizInput

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public string LeftIcon { get; set; }

        [Parameter]
        public string RightIcon { get; set; }

        [Parameter]
        public Icons? LeftSVGIcon { get; set; }

        [Parameter]
        public Icons? RightSVGIcon { get; set; }

        [Parameter]
        public InputSizes Size { get; set; } = InputSizes.Medium;

        [Parameter]
        public bool HasOutline { get; set; } = true;

        [Parameter]
        public bool HasShadow { get; set; }

        [Parameter]
        public bool IsTransparent { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public string Width { get; set; }

        [Parameter]
        public ValidationErrorStyles ValidationErrorStyle { get; set; } = ValidationErrorStyles.Label;

        public bool IsValid => !_hasParsingErrors && _isValid;

        public string ValidationMessage => _hasParsingErrors ? _parsingErrors : _validationMessage;

        #endregion

        [Parameter]
        public Icons? HandleSVGIcon { get; set; }

        [Parameter]
        public List<Country> Countries { get; set; }

        [Parameter]
        public Country SelectedCountry
        {
            get
            {
                return _selectedCountry;
            }
            set
            {
                if (_selectedCountry == value)
                    return;

                _selectedCountry = value;
                _ = SelectedCountryChanged.InvokeAsync(_selectedCountry);
            }
        }

        [Parameter]
        public EventCallback<Country> SelectedCountryChanged { get; set; }

        [Parameter]
        public string MaximumHeight { get; set; }

        [Parameter]
        public PopupOpenDirections OpenDirection { get; set; } = PopupOpenDirections.Bottom;

        [Parameter]
        public string PopupClass { get; set; }

        #endregion

        #region METHODS

        protected void SetSelectedCountry(int id)
        {
            SelectedCountry = Countries.Where(a => a.Id == id).FirstOrDefault();
            _isOpen = false;
        }

        #endregion

        #region EVENTS

        protected async Task OnClickInput()
        {
            if (_clickHandled)
            {
                _clickHandled = false;
                return;
            }

            if (!IsDisabled)
            {
                if (!_isOpen)
                    await Open();
                else
                    _isOpen = false;
            }
        }

        protected Task OnInputKeyDownHandler(KeyboardEventArgs args)
        {
            //if (IsDisabled)
            //    return;

            //if (args.Key == null)
            //    return;

            //if (args.Key == "Tab")
            //    await InvokeVoidAsync("focusNext", _inputElement);

            //if (!_isOpen)
            //    await Open();

            ////If list has items.
            ////Get the index of the selected item.

            //int activeItemIndex = _popupContent.GetActiveItemIndex();
            //int listSize = _popupContent.GetListSize();

            //switch (args.Key)
            //{
            //    case "Enter":

            //        if (activeItemIndex == -1) //If not item was selected.
            //        {
            //            activeItemIndex = 0; //Select the first item.
            //        }
            //        else
            //        {
            //            //Set the value of the AutoComplete based on the selected item.
            //            var selectItem = _selectItems.Where(a => a.Value.ListItem == _popupContent.ActiveItem).Select(a => a.Value).FirstOrDefault();
            //            await SetSelectedItem(selectItem);

            //            //Close the popup.
            //            _isOpen = false;

            //            return;
            //        }

            //        break;

            //    case "ArrowDown":

            //        if (activeItemIndex == -1 || activeItemIndex == listSize - 1) //If not item was selected or the last item was selected.
            //        {
            //            //Select the first item.
            //            activeItemIndex = 0;
            //        }
            //        else
            //        {
            //            //Select the next item.
            //            activeItemIndex += 1;
            //        }

            //        break;
            //    case "ArrowUp":

            //        if (activeItemIndex == -1 || activeItemIndex == 0) //If not item was selected or the first item was selected.
            //        {
            //            //Select the last item.
            //            activeItemIndex = listSize - 1;
            //        }
            //        else
            //        {
            //            //Select the previous item.
            //            activeItemIndex -= 1;
            //        }

            //        break;

            //    default:
            //        return;
            //}

            ////Update the selected item in the list.
            //await _popupContent.SetActiveItemIndex(activeItemIndex);

            return Task.CompletedTask;
        }

        public Task OnInputHandler(ChangeEventArgs args)
        {
            //_text = (string)args.Value;

            //if (SearchFunction != null)
            //{
            //    if (MinimumCharacters > 0 && _text.Length >= MinimumCharacters)
            //    {
            //        _deferredAction.Defer(_delayTimeSpan);
            //    }
            //}

            //StateHasChanged();
            return Task.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override void OnParametersSet()
        {
            if (EditContext != _lastEditContext && EditContext != null)
            {
                _validationMessageStore = new ValidationMessageStore(EditContext);
            }

            base.OnParametersSet();
        }

        public override void Validate()
        {
            _validationMessageStore.Clear();

            if (_hasParsingErrors)
            {
                _validationMessageStore.Add(_fieldIdentifier, _parsingErrors);
            }
        }

        #endregion

        #region METHODS

        private async Task Open()
        {
            if (OpenDirection == PopupOpenDirections.Cursor)
            {
                var windowSize = await JsInvokeAsync<WindowSize>("getWindowSize");
                var popupContentSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", _popupContent.Ref);

                var inputSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", _inputElement);

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
                 .If(PopupClass, () => !string.IsNullOrEmpty(PopupClass))
                 .AsString();

        protected string PopupStyleValue => new StyleMapper()
                 .If($"top: {_popupY.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If($"left: {_popupX.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If($"width: {_popupWidth.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .AsString();

        #endregion

    }
}