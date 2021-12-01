using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class NumericUpDown<TValue> : GizInputBase<TValue>
    {
        #region FIELDS

        private StringConverter<TValue> _converter = new StringConverter<TValue>();
        private string _text;

        #endregion

        #region PROPERTIES

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
        public CultureInfo? Culture { get; set; }

        [Parameter]
        public string Format { get; set; }

        public bool IsValid => _isValid && !_converter.HasGetError;

        public string ValidationMessage => _converter.HasGetError ? _converter.GetErrorMessage : _validationMessage;

        #endregion

        #region EVENTS

        public Task OnInputHandler(ChangeEventArgs args)
        {
            var newText = args?.Value as string;

            TValue newValue = _converter.GetValue(newText);

            if (!EqualityComparer<TValue>.Default.Equals(Value, newValue))
            {
                return SetValueAsync(newValue);
            }

            return Task.CompletedTask;
        }

        public Task OnClickButtonDecreaseValueHandler(MouseEventArgs args)
        {
            //Convert value
            decimal? value = ValueToDecimal();
            if (value.HasValue)
            {
                if (value - Step < Minimum)
                    return Task.CompletedTask;

                //Decrease value
                value -= Step;

                //Set new value
                return SetValueAsync(_converter.GetValue(value.ToString()));
            }

            return Task.CompletedTask;
        }

        public Task OnClickButtonIncreaseValueHandler(MouseEventArgs args)
        {
            //Convert value
            decimal? value = ValueToDecimal();
            if (value.HasValue)
            {
                if (value + Step > Maximum)
                    return Task.CompletedTask;

                //Increase value
                value += Step;

                //Set new value
                return SetValueAsync(_converter.GetValue(value.ToString()));
            }

            return Task.CompletedTask;
        }

        #endregion

        #region METHODS

        protected async Task SetValueAsync(TValue value)
        {
            Value = value;
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

        #region OVERRIDE

        protected override void OnInitialized()
        {
            if (Culture != null)
            {
                _converter.Culture = Culture;
            }

            base.OnInitialized();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var valueChanged = parameters.TryGetValue<TValue>(nameof(Value), out var newValue);
            if (valueChanged)
            {
                _text = _converter.SetValue(Value);
            }
        }

        #endregion

    }
}