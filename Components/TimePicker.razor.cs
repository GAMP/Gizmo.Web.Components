using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class TimePicker<TValue> : GizInputBase<TValue>
    {
        #region CONSTRUCTOR
        public TimePicker()
        {
            //Set default culture and format;
            _culture = CultureInfo.CurrentCulture;
            _format = _culture.DateTimeFormat.ShortDatePattern;
            _converter = new DateConverter<TValue>();
            _converter.Culture = _culture;
            _converter.Format = _format;
        }
        #endregion

        #region FIELDS

        private CultureInfo _culture;
        private string _format;
        private DateConverter<TValue> _converter;
        private string _text;
        private TimePickerBase<TValue> _popupContent;
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
        public TValue Value { get; set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;

                if (!_isOpen)
                    _popupContent.ReloadValue();
            }
        }

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

        [Parameter]
        public PopupOpenDirections OpenDirection { get; set; } = PopupOpenDirections.Bottom;

        [Parameter]
        public CultureInfo Culture { get; set; }

        [Parameter]
        public string Format { get; set; }

        [Parameter]
        public bool CanClearValue { get; set; }

        public bool IsValid => !_hasParsingErrors && _isValid && !_converter.HasGetError;

        public string ValidationMessage => _hasParsingErrors ? _parsingErrors : _converter.HasGetError ? _converter.GetErrorMessage : _validationMessage;

        #endregion

        #region EVENTS

        public Task OnInputHandler(ChangeEventArgs args)
        {
            //Get input text.
            var newText = args?.Value as string;

            //Try parse.
            try
            {
                var newDate = DateTime.ParseExact(newText, _format, _culture, DateTimeStyles.None);

                TValue newValue = _converter.GetValue(newDate);

                return SetValueAsync(newValue);
            }
            catch
            {
                _hasParsingErrors = true;
                _parsingErrors = "The field should be a time.";
            }

            return Task.CompletedTask;
        }

        protected async Task OnClickInput()
        {
            if (!IsDisabled)
            {
                if (!IsOpen && OpenDirection == PopupOpenDirections.Cursor)
                {
                    var windowSize = await JsInvokeAsync<WindowSize>("getWindowSize");
                    var mainMenuSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", _popupContent.Ref);

                    var inputSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", Ref);

                    _popupX = inputSize.Left;
                    _popupWidth = inputSize.Width;

                    if (inputSize.Bottom + mainMenuSize.Height > windowSize.Height)
                    {
                        _popupY = windowSize.Height - mainMenuSize.Height;
                    }
                    else
                    {
                        _popupY = inputSize.Bottom;
                    }
                }

                IsOpen = !IsOpen;
            }
        }

        protected Task OnClickOverlayHandler(MouseEventArgs args)
        {
            IsOpen = false;

            return Task.CompletedTask;
        }

        private Task TimePickerValueChanged(TValue value)
        {
            IsOpen = false;

            return SetValueAsync(value);
        }

        protected void OnClickOKButtonHandler()
        {
            IsOpen = false;
        }

        protected void OnClickCancelButtonHandler()
        {
            IsOpen = false;
        }

        public Task OnClickButtonClearValueHandler(MouseEventArgs args)
        {
            return SetValueAsync(default(TValue));
        }

        #endregion

        #region METHODS

        protected async Task SetValueAsync(TValue value)
        {
            _hasParsingErrors = false;
            _parsingErrors = String.Empty;

            if (!EqualityComparer<TValue>.Default.Equals(Value, value))
            {
                Value = value;

                await ValueChanged.InvokeAsync(Value);
            }
        }

        private bool IsNullable()
        {
            if (Nullable.GetUnderlyingType(typeof(TValue)) != null)
                return true;

            return false;
        }

        #endregion

        #region OVERRIDE

        protected override async Task OnParametersSetAsync()
        {
            if (Culture != null)
            {
                _culture = Culture;
            }
            else
            {
                _culture = CultureInfo.CurrentCulture;
            }

            if (!string.IsNullOrEmpty(Format))
            {
                _format = Format;
            }
            else
            {
                _format = _culture.DateTimeFormat.ShortTimePattern;
            }

            _converter.Culture = _culture;
            _converter.Format = _format;

            await base.OnParametersSetAsync();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var valueChanged = parameters.TryGetValue<TValue>(nameof(Value), out var newValue);
            if (valueChanged)
            {
                //Update the component's text.
                var value = _converter.SetValue(Value);
                if (value != null)
                {
                    _text = value.Value.ToString(_format, _culture);
                }
                else
                {
                    _text = string.Empty;
                }
            }
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

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-input-timepicker")
                 .If("giz-input-timepicker--full-width", () => IsFullWidth)
                 .Add("giz-input-timepicker--popup")
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-input-timepicker__dropdown")
                 .If("giz-input-datepicker__dropdown--cursor", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If("giz-timepicker-dropdown-full-width", () => OpenDirection != PopupOpenDirections.Cursor)
                 .AsString();

        protected string PopupStyleValue => new StyleMapper()
                 .If($"top: {_popupY.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If($"left: {_popupX.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .AsString();

        #endregion

    }
}