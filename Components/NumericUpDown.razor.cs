using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class NumericUpDown<TValue> : GizInputBase<TValue>
    {
        #region CONSTRUCTOR
        public NumericUpDown()
        {
            //Set default culture and format;
            _culture = CultureInfo.CurrentCulture;
            _converter = new StringConverter<TValue>();
            _converter.Culture = _culture;
        }
        #endregion

        #region FIELDS

        private CultureInfo _culture;
        private StringConverter<TValue> _converter;
        private string _text;
        private decimal? _decimalValue;
        private ElementReference _inputElement;
        private ValidationMessageStore _validationMessageStore;

        #endregion

        #region PROPERTIES

        [Parameter]
        public ValidationErrorStyles ValidationErrorStyle { get; set; } = ValidationErrorStyles.Label;

        [Parameter]
        public decimal Minimum { get; set; } = 0;

        [Parameter]
        public decimal Maximum { get; set; } = decimal.MaxValue;

        [Parameter]
        public decimal Step { get; set; } = 1;

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public string Label { get; set; }

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
        public CultureInfo Culture { get; set; }

        [Parameter]
        public string Format { get; set; }

        [Parameter]
        public bool CanClearValue { get; set; }

        public bool IsValid => _isValid && !_converter.HasGetError;

        public string ValidationMessage => _converter.HasGetError ? _converter.GetErrorMessage : _validationMessage;

        #endregion

        #region EVENTS

        public Task OnInputHandler(ChangeEventArgs args)
        {
            var newText = args?.Value as string;

            TValue newValue = _converter.GetValue(newText);

            if (!_converter.HasGetError)
            {
                if (!EqualityComparer<TValue>.Default.Equals(Value, newValue))
                {
                    return SetValueAsync(newValue);
                }
            }

            return Task.CompletedTask;
        }

        public Task OnClickButtonDecreaseValueHandler(MouseEventArgs args)
        {
            //Convert value
            decimal? value = ValueToDecimal();
            if (!value.HasValue)
                value = 0;

            if (value - Step < Minimum)
                return Task.CompletedTask;

            //Decrease value
            value -= Step;

            //Set new value
            return SetValueAsync(_converter.GetValue(value.ToString()));
        }

        public Task OnClickButtonIncreaseValueHandler(MouseEventArgs args)
        {
            //Convert value
            decimal? value = ValueToDecimal();
            if (!value.HasValue)
                value = 0;

            if (value + Step > Maximum)
                return Task.CompletedTask;

            //Increase value
            value += Step;

            //Set new value
            return SetValueAsync(_converter.GetValue(value.ToString()));
        }

        public Task OnClickButtonClearValueHandler(MouseEventArgs args)
        {
            return SetValueAsync(default(TValue));
        }

        #endregion

        #region METHODS

        protected async Task SetValueAsync(TValue value)
        {
            Value = value;

            _decimalValue = ValueToDecimal();

            await ValueChanged.InvokeAsync(Value);
        }

        private decimal? ValueToDecimal()
        {
            decimal? result = null;

            if (Value == null)
                return result;

            if (typeof(TValue) == typeof(short) || typeof(TValue) == typeof(short?))
                return System.Convert.ToDecimal((short)(object)Value);

            if (typeof(TValue) == typeof(int) || typeof(TValue) == typeof(int?))
                return System.Convert.ToDecimal((int)(object)Value);

            if (typeof(TValue) == typeof(long) || typeof(TValue) == typeof(long?))
                return System.Convert.ToDecimal((long)(object)Value);

            if (typeof(TValue) == typeof(float) || typeof(TValue) == typeof(float?))
                return System.Convert.ToDecimal((float)(object)Value);

            if (typeof(TValue) == typeof(double) || typeof(TValue) == typeof(double?))
                return System.Convert.ToDecimal((double)(object)Value);

            if (typeof(TValue) == typeof(decimal) || typeof(TValue) == typeof(decimal?))
                return (decimal)(object)Value;

            return result;
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            if (Culture != null)
            {
                _converter.Culture = Culture;
            }

            base.OnInitialized();
        }

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

            _converter.Culture = _culture;

            if (EditContext != _lastEditContext && EditContext != null)
            {
                _validationMessageStore = new ValidationMessageStore(EditContext);
            }

            base.OnParametersSet();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var valueChanged = parameters.TryGetValue<TValue>(nameof(Value), out var newValue);
            if (valueChanged)
            {
                _text = _converter.SetValue(Value);

                _decimalValue = ValueToDecimal();

                StateHasChanged();
            }
        }

        public override void Validate()
        {
            if (_validationMessageStore != null)
            {
                _validationMessageStore.Clear();

                if (_converter.HasGetError)
                {
                    _validationMessageStore.Add(_fieldIdentifier, _converter.GetErrorMessage);
                }
            }
        }

        private bool IsNullable()
        {
            if (Nullable.GetUnderlyingType(typeof(TValue)) != null)
                return true;

            return false;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-numeric-up-down")
                 .If("giz-numeric-up-down--full-width", () => IsFullWidth)
                 .If("giz-numeric-up-down--formatted", () => !string.IsNullOrEmpty(Format) && !_converter.HasGetError)
                 .If("giz-numeric-up-down--formatted--invalid", () => !string.IsNullOrEmpty(Format) && _converter.HasGetError)
                 .Add(Class)
                 .AsString();

        #endregion

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
