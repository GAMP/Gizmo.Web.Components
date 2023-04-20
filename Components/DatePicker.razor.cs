using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class DatePicker<TValue> : GizInputBase<TValue>, IGizInput
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
        private ValidationMessageStore _validationMessageStore;

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

        public bool IsValid => !_hasParsingErrors && _isValid && !_converter.HasGetError;

        public string ValidationMessage => _hasParsingErrors ? _parsingErrors : _converter.HasGetError ? _converter.GetErrorMessage : _validationMessage;

        #endregion

        [Parameter]
        public PickerVariants Variant { get; set; } = PickerVariants.Inline;

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public bool OffsetY { get; set; }

        [Parameter]
        public PopupOpenDirections OpenDirection { get; set; } = PopupOpenDirections.Bottom;

        [Parameter]
        public bool ShowTime { get; set; }

        [Parameter]
        public CultureInfo Culture { get; set; }

        [Parameter]
        public string Format { get; set; }

        [Parameter]
        public bool CanClearValue { get; set; }

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

        public Task OnClickButtonClearValueHandler(MouseEventArgs args)
        {
            return SetValueAsync(default(TValue));
        }

        #endregion

        #region METHODS

        protected async Task SetValueAsync(TValue value)
        {
            _hasParsingErrors = false;
            _parsingErrors = string.Empty;

            if (!EqualityComparer<TValue>.Default.Equals(Value, value))
            {
                Value = value;

                await ValueChanged.InvokeAsync(Value);
                NotifyFieldChanged();
            }
        }

        private bool IsNullable()
        {
            if (Nullable.GetUnderlyingType(typeof(TValue)) != null)
                return true;

            return false;
        }

        #endregion

        #region OVERRIDES

        protected override void OnParametersSet()
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

            if (EditContext != _lastEditContext && EditContext != null)
            {
                _validationMessageStore = new ValidationMessageStore(EditContext);
            }

            base.OnParametersSet();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<TValue>(nameof(Value), out var newValue))
            {
                var valueChanged = !EqualityComparer<TValue>.Default.Equals(Value, newValue);
                if (valueChanged)
                {
                    //Update the component's text.
                    DateTime? value = _converter.SetValue(newValue);

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

            await base.SetParametersAsync(parameters);
        }

        public override void Validate()
        {
            if (_validationMessageStore != null)
            {
                _validationMessageStore.Clear();

                if (_hasParsingErrors)
                {
                    _validationMessageStore.Add(_fieldIdentifier, _parsingErrors);
                }
            }
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-date-picker-input")
                 .If("giz-date-picker-input--full-width", () => IsFullWidth)
                 .If("giz-date-picker-input--dialog", () => Variant == PickerVariants.Dialog)
                 .If("giz-date-picker-input--popup", () => Variant == PickerVariants.Inline)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-date-picker-input__dropdown")
                 .If("giz-date-picker-input__dropdown--cursor", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If("giz-date-picker-input__dropdown--full-width", () => OpenDirection != PopupOpenDirections.Cursor)
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
