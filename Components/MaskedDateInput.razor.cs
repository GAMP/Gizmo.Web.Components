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
    public partial class MaskedDateInput<TValue> : MaskedInputBase<TValue>
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

            _tempConverter = new DateConverter<string>();
            _tempConverter.Culture = _culture;
            _tempConverter.Format = _format;

            _separator = _culture.DateTimeFormat.DateSeparator[0];

            var expandedFormat = _format;

            if (!expandedFormat.Contains("dd"))
            {
                expandedFormat = expandedFormat.Replace("d", "dd");
            }

            if (!expandedFormat.Contains("MM"))
            {
                expandedFormat = expandedFormat.Replace("M", "MM");
            }

            _mask = expandedFormat;
            _chars = _mask.Count();
            _mask_left = _mask;
        }
        #endregion

        #region FIELDS

        private CultureInfo _culture;
        private string _format;
        private DateConverter<TValue> _converter;
        private DateConverter<string> _tempConverter;
        private char _separator;

        private bool _shouldRender;
        private string _previousMask = string.Empty;
        private TValue _previousValue;

        private string _mask;
        private int _chars = 0;

        private bool _hasParsingErrors;
        private string _parsingErrors;
        private ValidationMessageStore _validationMessageStore;

        #endregion

        #region PROPERTIES

        public new bool IsValid => !_hasParsingErrors && _isValid && !_converter.HasGetError;

        public new string ValidationMessage => _hasParsingErrors ? _parsingErrors : _converter.HasGetError ? _converter.GetErrorMessage : _validationMessage;

        #endregion

        #region EVENTS

        protected new async Task OnInputKeyDownHandler(KeyboardEventArgs args)
        {
            if (IsDisabled)
                return;

            if (args.Key == null)
                return;

            if (args.Key == "Tab")
                await InvokeVoidAsync("focusNext", _inputElement);

            if (args.Key == _separator.ToString())
            {
                //TODO: A move to next block
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

                        if (_text.Length == _chars)
                        {
                            DateTime? temp = _tempConverter.SetValue(_text);
                            if (!_tempConverter.HasSetError)
                            {
                                await SetValueAsync(_converter.GetValue(temp));
                            }
                            else
                            {
                                _hasParsingErrors = true;
                                _parsingErrors = "The field should be a date.";
                            }
                        }
                        else
                        {
                            if (_mask[_text.Length] == _separator)
                            {
                                _text += _separator;
                            }
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

        protected async Task OnMouseUpHandler(MouseEventArgs args)
        {
            await InvokeVoidAsync("dropSelection", _inputElement);
        }

        protected async Task OnFocusHanlder()
        {
            await InvokeVoidAsync("dropSelection", _inputElement);
        }

        #endregion

        #region METHODS

        protected new async Task SetValueAsync(TValue value)
        {
            _hasParsingErrors = false;
            _parsingErrors = String.Empty;
        
            if (!EqualityComparer<TValue>.Default.Equals(Value, value))
            {
                Value = value;

                await ValueChanged.InvokeAsync(Value);
                NotifyFieldChanged();
            }
        }

        #endregion

        #region OVERRIDES

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

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-masked-date-input")
                 .Add("giz-input-text")
                 .If("giz-input-text--full-width", () => IsFullWidth)
                 .Add(Class)
                 .AsString();

        #endregion

    }
}