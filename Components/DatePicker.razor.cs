using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class DatePicker<TValue> : GizInputBase<TValue>
    {
        #region CONSTRUCTOR
        public DatePicker()
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
        private DatePickerBase<TValue> _popupContent;
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
        public PickerVariants Variant { get; set; } = PickerVariants.Inline;

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public string Label { get; set; }

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
        public bool ShowTime { get; set; }

        [Parameter]
        public CultureInfo Culture { get; set; }

        [Parameter]
        public string Format { get; set; }

        public bool IsValid => !_hasParsingErrors && _isValid && !_converter.HasGetError;

        public string ValidationMessage => _hasParsingErrors ? _parsingErrors : _converter.HasGetError ? _converter.GetErrorMessage : _validationMessage;

        #endregion

        #region EVENTS

        private Task DatePickerValueChanged(TValue value)
        {
            _isOpen = false;

            return SetValueAsync(value);
        }

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
                _parsingErrors = "The field should be a date.";
            }

            return Task.CompletedTask;
        }

        protected async Task OnClickInput()
        {
            if (!IsDisabled)
            {
                if (!_isOpen && OpenDirection == PopupOpenDirections.Cursor)
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

                _isOpen = !_isOpen;
            }
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
                if (ShowTime)
                {
                    _format = _culture.DateTimeFormat.ShortDatePattern + " " + _culture.DateTimeFormat.ShortTimePattern;
                }
                else
                {
                    _format = _culture.DateTimeFormat.ShortDatePattern;
                }
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
                DateTime? value = _converter.SetValue(Value);

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
                 .Add("giz-input-datepicker")
                 .If("giz-input-datepicker--full-width", () => IsFullWidth)
                 .If("giz-input-datepicker--dialog", () => Variant == PickerVariants.Dialog)
                 .If("giz-input-datepicker--popup", () => Variant == PickerVariants.Inline)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-input-datepicker__dropdown")
                 .If("giz-input-datepicker__dropdown--cursor", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If("giz-datepicker-dropdown-full-width", () => OpenDirection != PopupOpenDirections.Cursor)
                 .If("giz-popup--bottom", () => Variant == PickerVariants.Inline)
                 .If("giz-popup--offset", () => Variant == PickerVariants.Inline)
                 .AsString();

        protected string PopupStyleValue => new StyleMapper()
                 .If($"top: {_popupY.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If($"left: {_popupX.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .AsString();

        #endregion

    }
}