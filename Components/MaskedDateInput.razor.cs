using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class MaskedDateInput<TValue> : GizInputBase<TValue>
    {
        #region CONSTRUCTOR
        public MaskedDateInput()
        {
            //Set default culture and format;
            _culture = CultureInfo.CurrentCulture;
            _format = _culture.DateTimeFormat.ShortDatePattern;
            _converter = new DateConverter<TValue>();
            _converter.Culture = _culture;
            _converter.Format = _format;
            _separator = _culture.DateTimeFormat.DateSeparator[0];

            _mask = _format;
            _chars = _mask.Count();
            _mask_left = _mask;
        }
        #endregion

        private CultureInfo _culture;
        private string _format;
        private DateConverter<TValue> _converter;
        private char _separator;

        private bool _shouldRender;
        private string _previousMask = string.Empty;
        private TValue _previousValue;

        private string _mask;
        private int _chars = 0;
        private string _mask_left = string.Empty;

        #region FIELDS

        private string _text;

        #endregion

        #region PROPERTIES

        [Parameter]
        public ValidationErrorStyles ValidationErrorStyle { get; set; } = ValidationErrorStyles.Label;

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string LeftIcon { get; set; }

        [Parameter]
        public string RightIcon { get; set; }

        [Parameter]
        public Icons? LeftSVGIcon { get; set; }

        [Parameter]
        public Icons? RightSVGIcon { get; set; }

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
        public TValue Value { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public string Type { get; set; } = "text";

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public int Min { get; set; }

        [Parameter]
        public int Max { get; set; }

        [Parameter]
        public int MaxLength { get; set; }

        [Parameter]
        public CultureInfo Culture { get; set; }

        public bool IsValid => _isValid && !_converter.HasGetError;

        public string ValidationMessage => _converter.HasGetError ? _converter.GetErrorMessage : _validationMessage;

        #endregion

        #region EVENTS

        protected async Task OnInputKeyDownHandler(KeyboardEventArgs args)
        {

            if (args.Key == _separator.ToString())
            {
                //move to next block
            }
            else
            {
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

                        if (!string.IsNullOrEmpty(_text) && _text.Length >= _chars)
                            return;

                        _text += args.Key;

                        //if (textValue == _chars)
                        //await SetValueAsync(_converter.GetValue(currentValue));

                        if (_mask[_text.Length] == _separator)
                        {
                            _text += _separator;
                        }

                        break;

                    case "Backspace":

                        if (string.IsNullOrEmpty(_text) || _text.Length == 0)
                            return;

                        _text = _text.Substring(0, _text.Length - 1);
                        //set value to null?

                        if (_mask[_text.Length] == _separator)
                        {
                            _text = _text.Substring(0, _text.Length - 1);
                        }

                        break;
                }
            }

            if (!string.IsNullOrEmpty(_text) && _text.Length > 0)
            {
                _mask_left = _mask.Substring(_text.Length);
            }
            else
            {
                _mask_left = _mask;
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

        #region OVERRIDE

        protected override void OnInitialized()
        {
            if (Culture != null)
            {
                _converter.Culture = Culture;
            }

            base.OnInitialized();
        }

        protected override Task OnFirstAfterRenderAsync()
        {
            var attributes = new Dictionary<string, object>();

            if (Min > 0)
                attributes["min"] = Min;

            if (Max > 0)
                attributes["max"] = Max;

            if (MaxLength > 0)
                attributes["maxlength"] = MaxLength;

            Attributes = attributes;

            return base.OnFirstAfterRenderAsync();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-masked-date-input")
                 .Add("giz-input-text")
                 .If("giz-input-text--full-width", () => IsFullWidth)
                 .Add(Class)
                 .AsString();

        #endregion

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
    }
}