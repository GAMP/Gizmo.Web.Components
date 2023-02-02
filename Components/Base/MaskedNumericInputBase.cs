using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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
        private string _previousMask = string.Empty;
        private TValue _previousValue;

        private StringConverter<TValue> _converter = new StringConverter<TValue>();

        private bool _hasParsingErrors;
        private string _parsingErrors;
        private ValidationMessageStore _validationMessageStore;

        #endregion

        #region PROPERTIES

        #region IGizInput

        public override bool IsValid => !_hasParsingErrors && !_converter.HasGetError && _isValid;

        public override string ValidationMessage => _hasParsingErrors ? _parsingErrors : _converter.HasGetError ? _converter.GetErrorMessage : _validationMessage;

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
            {
                if (args.ShiftKey)
                {
                    await InvokeVoidAsync("focusPrevious", _inputElement);
                }
                else
                {
                    await InvokeVoidAsync("focusNext", _inputElement);
                }
                return;
            }

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
            UpdateText();
            await ValueChanged.InvokeAsync(Value);
            NotifyFieldChanged();
        }

        private void UpdateText()
        {
            var maskChanged = _previousMask != Mask;
            if (maskChanged)
            {
                _shouldRender = true;
                _previousMask = Mask;

                _chars = Mask.Where(a => a == '#').Count();
                _mask_left = Mask;
            }

            var valueChanged = !EqualityComparer<TValue>.Default.Equals(_previousValue, Value);
            if (valueChanged)
            {
                _shouldRender = true;
                _previousValue = Value;

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

                //Validate new value.
                if (!temp.All(char.IsDigit))
                {
                    _hasParsingErrors = true;
                    _parsingErrors = "The field should be a number.";
                }
                else
                {
                    _hasParsingErrors = false;
                    _parsingErrors = string.Empty;
                }
            }
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
            await base.SetParametersAsync(parameters);

            UpdateText();
        }

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
    }
}
