using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public class MaskedInputBase<TValue> : GizInputBase<TValue>, IGizInput
    {
        #region FIELDS

        private bool _shouldRender;
        private string _previousMask = string.Empty;
        private TValue _previousValue;

        private int _chars = 0;
        protected string _mask_left = string.Empty;

        private StringConverter<TValue> _converter = new StringConverter<TValue>();

        protected ElementReference _inputElement;
        protected string _text;

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
        public string Width { get; set; } = "20rem";

        [Parameter]
        public ValidationErrorStyles ValidationErrorStyle { get; set; } = ValidationErrorStyles.Label;

        public bool IsValid => _isValid && !_converter.HasGetError;

        public string ValidationMessage => _converter.HasGetError ? _converter.GetErrorMessage : _validationMessage;

        #endregion

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public CultureInfo Culture { get; set; }

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

        protected Task OnClickHandler(MouseEventArgs args)
        {
            return OnClick.InvokeAsync(args);
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
            await base.SetParametersAsync(parameters);

            if (parameters.TryGetValue<string>(nameof(Mask), out var newMask))
            {
                var maskChanged = _previousMask != Mask;
                if (maskChanged)
                {
                    _shouldRender = true;
                    _previousMask = Mask;

                    _chars = Mask.Where(a => a == '#').Count();
                    _mask_left = Mask;
                }
            }

            if (parameters.TryGetValue<TValue>(nameof(Value), out var newValue))
            {
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
                }
            }
        }

        //protected override bool ShouldRender()
        //{
        //    return _shouldRender;
        //}

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                _shouldRender = false;
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        #endregion
    }
}