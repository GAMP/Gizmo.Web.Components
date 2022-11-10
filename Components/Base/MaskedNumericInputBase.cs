using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public class MaskedNumericInputBase<TValue> : MaskedInputBase<TValue>, IGizInput
    {
        #region FIELDS

        private int _chars = 0;

        private StringConverter<TValue> _converter = new StringConverter<TValue>();

        #endregion

        #region PROPERTIES

        #region IGizInput
        
        public override bool IsValid => _isValid && !_converter.HasGetError;

        public override string ValidationMessage => _converter.HasGetError ? _converter.GetErrorMessage : _validationMessage;

        #endregion

        [Parameter]
        public string Mask { get; set; }

        #endregion

        #region EVENTS

        protected async Task OnInputKeyDownHandler(KeyboardEventArgs args)
        {
            if (IsDisabled)
                return;

            if (args.Key == null)
                return;

            if (args.Key == "Tab")
                await InvokeVoidAsync("focusNext", _inputElement);

            //var inputSelectionRange = await JsInvokeAsync<InputSelectionRange>("getInputSelectionRange");

            var currentValue = _converter.SetValue(Value);

            switch (args.Key)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":

                    if (!string.IsNullOrEmpty(currentValue) && currentValue.Length >= _chars)
                        return;

                    currentValue += args.Key;
                    await SetValueAsync(_converter.GetValue(currentValue));

                    break;

                case "Backspace":

                    if (string.IsNullOrEmpty(currentValue) || currentValue.Length == 0)
                        return;

                    currentValue = currentValue.Substring(0, currentValue.Length - 1);
                    await SetValueAsync(_converter.GetValue(currentValue));

                    break;
            }
        }

        #endregion

        #region METHODS

        protected async Task SetValueAsync(TValue value)
        {
            Value = value;
            await ValueChanged.InvokeAsync(Value);
            NotifyFieldChanged();
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

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<string>(nameof(Mask), out var newMask))
            {
                var maskChanged = Mask != newMask;
                if (maskChanged)
                {
                    _shouldRender = true;

                    _chars = Mask.Where(a => a == '#').Count();
                    _mask_left = Mask;
                }
            }

            if (parameters.TryGetValue<TValue>(nameof(Value), out var newValue))
            {
                var valueChanged = !EqualityComparer<TValue>.Default.Equals(Value, newValue);
                if (valueChanged)
                {
                    _shouldRender = true;

                    _text = string.Empty;

                    var temp = _converter.SetValue(Value);

                    if (!string.IsNullOrEmpty(temp) && temp.Length > 0)
                    {
                        int additionalCharacters = 0;

                        for (int i = 0; i < Mask.Length; i++)
                        {
                            if (i > temp.Length - 1 + additionalCharacters)
                                break;

                            if (Mask[i] == '#')
                            {
                                _text += temp[i - additionalCharacters];
                            }
                            else
                            {
                                _text += Mask[i];
                                additionalCharacters += 1;
                            }
                        }

                        _mask_left = Mask.Substring(temp.Length + additionalCharacters);
                    }
                    else
                    {
                        _text = string.Empty;
                        _mask_left = Mask;
                    }
                }
            }

            await base.SetParametersAsync(parameters);
        }

        #endregion
    }
}