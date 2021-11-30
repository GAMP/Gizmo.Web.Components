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

        public Task OnClickButtonRemoveQuantityHandler(MouseEventArgs args)
        {
            return Task.CompletedTask;
            //decimal value = _converter.SetValue(Value);
            //if (value - Step < Minimum)
            //    return Task.CompletedTask;

            //value -= Step;
            //return SetValueAsync(_converter.GetValue(value.ToString()));
        }

        public Task OnClickButtonAddQuantityHandler(MouseEventArgs args)
        {
            return Task.CompletedTask;
            //decimal value = _converter.SetValue(Value);
            //if (value + Step > Maximum)
            //    return Task.CompletedTask;

            //value += Step;
            //return SetValueAsync(_converter.GetValue(value.ToString()));
        }

        #endregion

        #region METHODS

        protected async Task SetValueAsync(TValue value)
        {
            Value = value;
            await ValueChanged.InvokeAsync(Value);
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