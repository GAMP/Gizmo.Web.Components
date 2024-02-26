using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
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
        private ElementReference _inputElement;
        private ValidationMessageStore _validationMessageStore;

        private bool _altPressed;

        protected bool _shouldRender;

        //private decimal? _decimalValue;

        #endregion

        #region PROPERTIES

        [Inject]
        private ILogger<NumericUpDown<TValue>> Logger { get; set; } = null!;

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

        protected async Task OnInputKeyDownHandler(KeyboardEventArgs args)
        {
            if (IsDisabled)
                return;

            try
            {
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

                if (_altPressed)
                {
                    if (!args.AltKey)
                    {
                        _altPressed = false;
                    }
                }
                else
                {
                    if (args.Key == "Alt")
                    {
                        _altPressed = true;
                    }
                }

                if (_altPressed)
                    return;

                var previousValue = _text; //_converter.SetValue(Value);

                var inputSelectionRange = await JsInvokeAsync<InputSelectionRange>("getInputSelectionRange", _inputElement);

                int caretIndex = 0;

                string part1 = string.Empty;
                string part2 = string.Empty;

                switch (args.Key)
                {
                    case "Home":
                        caretIndex = 0;
                        await JsInvokeAsync<InputSelectionRange>("setInputCaretIndex", _inputElement, caretIndex);

                        break;

                    case "End":
                        if (!string.IsNullOrEmpty(previousValue))
                        {
                            caretIndex = previousValue.Length;
                            await JsInvokeAsync<InputSelectionRange>("setInputCaretIndex", _inputElement, caretIndex);
                        }

                        break;

                    case "ArrowLeft":
                        caretIndex = inputSelectionRange.SelectionStart;

                        if (caretIndex > 0)
                        {
                            caretIndex -= 1;
                            await JsInvokeAsync<InputSelectionRange>("setInputCaretIndex", _inputElement, caretIndex);
                        }

                        break;

                    case "ArrowRight":
                        caretIndex = inputSelectionRange.SelectionStart;

                        if (caretIndex < previousValue.Length)
                        {
                            caretIndex += 1;
                            await JsInvokeAsync<InputSelectionRange>("setInputCaretIndex", _inputElement, caretIndex);
                        }

                        break;

                    case "Backspace":

                        caretIndex = inputSelectionRange.SelectionStart;

                        if (inputSelectionRange.SelectionStart != inputSelectionRange.SelectionEnd)
                        {
                            //If there is a selection.
                            if (inputSelectionRange.SelectionStart > 0)
                            {
                                //Get part before selection.
                                part1 = previousValue.Substring(0, inputSelectionRange.SelectionStart);
                            }

                            if (inputSelectionRange.SelectionEnd < previousValue.Length)
                            {
                                //Get part after selection.
                                part2 = previousValue.Substring(inputSelectionRange.SelectionEnd, previousValue.Length - inputSelectionRange.SelectionEnd);
                            }

                            //The selection is removed.
                            string tmp = part1 + part2;
                            TValue newValue = _converter.GetValue(tmp);

                            await SetValueAndTextAsync(newValue, tmp);
                        }
                        else
                        {
                            //If there is no selection.
                            if (inputSelectionRange.SelectionStart > 0)
                            {
                                //Get part before caret -1 character.
                                part1 = previousValue.Substring(0, inputSelectionRange.SelectionStart - 1);
                            }

                            if (!string.IsNullOrEmpty(previousValue) && inputSelectionRange.SelectionStart < previousValue.Length)
                            {
                                //Get part after caret.
                                part2 = previousValue.Substring(inputSelectionRange.SelectionStart, previousValue.Length - inputSelectionRange.SelectionStart);
                            }

                            string tmp = part1 + part2;
                            TValue newValue = _converter.GetValue(tmp);

                            await SetValueAndTextAsync(newValue, tmp);

                            caretIndex -= 1;
                        }

                        _shouldRender = true;
                        await InvokeAsync(StateHasChanged);

                        await JsInvokeAsync<InputSelectionRange>("setInputCaretIndex", _inputElement, caretIndex);

                        break;

                    case "Delete":

                        caretIndex = inputSelectionRange.SelectionStart;

                        if (inputSelectionRange.SelectionStart != inputSelectionRange.SelectionEnd)
                        {
                            //If there is a selection.
                            if (inputSelectionRange.SelectionStart > 0)
                            {
                                //Get part before selection.
                                part1 = previousValue.Substring(0, inputSelectionRange.SelectionStart);
                            }

                            if (inputSelectionRange.SelectionEnd < previousValue.Length)
                            {
                                //Get part after selection.
                                part2 = previousValue.Substring(inputSelectionRange.SelectionEnd, previousValue.Length - inputSelectionRange.SelectionEnd);
                            }

                            //The selection is removed.
                            string tmp = part1 + part2;
                            TValue newValue = _converter.GetValue(tmp);

                            await SetValueAndTextAsync(newValue, tmp);
                        }
                        else
                        {
                            //If there is no selection.
                            if (inputSelectionRange.SelectionStart > 0)
                            {
                                //Get part before caret character.
                                part1 = previousValue.Substring(0, inputSelectionRange.SelectionStart);
                            }

                            if (!string.IsNullOrEmpty(previousValue) && inputSelectionRange.SelectionStart + 1 < previousValue.Length)
                            {
                                //Get part after caret + 1.
                                part2 = previousValue.Substring(inputSelectionRange.SelectionStart + 1, previousValue.Length - (inputSelectionRange.SelectionStart + 1));
                            }

                            string tmp = part1 + part2;
                            TValue newValue = _converter.GetValue(tmp);

                            await SetValueAndTextAsync(newValue, tmp);

                            //caretIndex -= 1;
                        }

                        _shouldRender = true;
                        await InvokeAsync(StateHasChanged);

                        await JsInvokeAsync<InputSelectionRange>("setInputCaretIndex", _inputElement, caretIndex);

                        break;

                    default:

                        if (args.Key.Length == 1 && !char.IsControl(args.Key[0]))
                        {
                            caretIndex = inputSelectionRange.SelectionStart;

                            bool isValidCharacter = ValidateCharacterFunction(args.Key[0]);

                            if (isValidCharacter)
                            {
                                if (inputSelectionRange.SelectionStart != inputSelectionRange.SelectionEnd)
                                {
                                    //If there is a selection.
                                    if (inputSelectionRange.SelectionStart > 0)
                                    {
                                        //Get part before selection.
                                        part1 = previousValue.Substring(0, inputSelectionRange.SelectionStart);
                                    }

                                    if (inputSelectionRange.SelectionEnd < previousValue.Length)
                                    {
                                        //Get part after selection.
                                        part2 = previousValue.Substring(inputSelectionRange.SelectionEnd, previousValue.Length - inputSelectionRange.SelectionEnd);
                                    }

                                    string tmp = part1 + args.Key + part2;

                                    //Replace selection with new character.
                                    TValue newValue = _converter.GetValue(part1 + args.Key + part2);

                                    if (!_converter.HasGetError)
                                    {
                                        await SetValueAndTextAsync(newValue, tmp);
                                    }
                                    else
                                    {
                                        isValidCharacter = false;
                                    }
                                }
                                else
                                {
                                    //If there is no selection.
                                    if (inputSelectionRange.SelectionStart > 0)
                                    {
                                        //Get part before caret.
                                        part1 = previousValue.Substring(0, inputSelectionRange.SelectionStart);
                                    }

                                    if (!string.IsNullOrEmpty(previousValue) && inputSelectionRange.SelectionStart < previousValue.Length)
                                    {
                                        //Get part after caret.
                                        part2 = previousValue.Substring(inputSelectionRange.SelectionStart, previousValue.Length - inputSelectionRange.SelectionStart);
                                    }

                                    string tmp = part1 + args.Key + part2;

                                    //Insert new character between.
                                    TValue newValue = _converter.GetValue(tmp);

                                    if (!_converter.HasGetError)
                                    {
                                        await SetValueAndTextAsync(newValue, tmp);
                                    }
                                    else
                                    {
                                        isValidCharacter = false;
                                    }
                                }

                                if (isValidCharacter)
                                {
                                    _shouldRender = true;
                                    await InvokeAsync(StateHasChanged);

                                    caretIndex += 1;
                                    await JsInvokeAsync<InputSelectionRange>("setInputCaretIndex", _inputElement, caretIndex);
                                }
                            }
                        }

                        break;
                }
            }
            catch (NullReferenceException nullRef)
            {
                Logger.LogCritical(nullRef, "Null reference in input handler.");
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Error in input handler.");
            }
        }

        protected Task OnInputKeyUpHandler(KeyboardEventArgs args)
        {
            return Task.CompletedTask;
        }

        protected Task OnInputKeyPressHandler(KeyboardEventArgs args)
        {
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
            var tmpText = value.ToString();
            var tmpValue = _converter.GetValue(tmpText);
            return SetValueAndTextAsync(tmpValue, tmpText);
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
            var tmpText = value.ToString();
            var tmpValue = _converter.GetValue(tmpText);
            return SetValueAndTextAsync(tmpValue, tmpText);
        }

        public async Task OnClickButtonClearValueHandler(MouseEventArgs args)
        {
            await SetValueAndTextAsync(default(TValue), string.Empty);

            _shouldRender = true;
        }

        #endregion

        #region METHODS

        protected virtual async Task SetValueAndTextAsync(TValue value, string text)
        {
            Value = value;
            await ValueChanged.InvokeAsync(Value);
            NotifyFieldChanged();
            _text = text;
            //_decimalValue = ValueToDecimal();
        }

        public bool ValidateCharacterFunction(char character)
        {
            if (char.IsNumber(character))
                return true;

            if ((typeof(TValue) == typeof(decimal) || typeof(TValue) == typeof(decimal?)) &&
                CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator.Equals(character.ToString(), StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
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

            _converter.Culture = _culture;

            if (EditContext != _lastEditContext && EditContext != null)
            {
                _validationMessageStore = new ValidationMessageStore(EditContext);
            }

            TValue previousValue = _converter.GetValue(_text);

            if (!EqualityComparer<TValue>.Default.Equals(previousValue, Value))
            {
                _text = _converter.SetValue(Value); //TODO: A CURRENCY DECIMALS
                //_decimalValue = ValueToDecimal();
            }

            base.OnParametersSet();
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

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                _shouldRender = false;
                //await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
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
    }
}
