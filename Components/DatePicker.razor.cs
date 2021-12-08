using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class DatePicker : GizInputBase<DateTime?>
    {
        #region CONSTRUCTOR
        public DatePicker()
        {
        }
        #endregion

        #region FIELDS

        private DateTime? _value;
        private string _text;
        private DatePickerBase _popupContent;
        private bool _isOpen;
        private double _popupX;
        private double _popupY;
        private double _popupWidth;

        #endregion

        #region PROPERTIES

        [Parameter]
        public PickerVariants Variant { get; set; } = PickerVariants.Inline;

        [Parameter]
        public DateTime? Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value == value)
                    return;

                _value = value;

                //Update the component's text.
                if (_value != null)
                {
                    if (ShowTime)
                        _text = _value.Value.ToString();
                    else
                        _text = _value.Value.ToShortDateString();
                }
                else
                {
                    _text = string.Empty;
                }

                ValueChanged.InvokeAsync(Value);
            }
        }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public bool OffsetY { get; set; }

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
        public string Placeholder { get; set; }

        [Parameter]
        public PopupOpenDirections OpenDirection { get; set; } = PopupOpenDirections.Bottom;

        [Parameter]
        public bool ShowTime { get; set; }

        #endregion

        #region EVENTS

        private Task DatePickerValueChanged(DateTime? value)
        {
            _isOpen = false;
            Value = value;

            return Task.CompletedTask;
        }

        public Task OnInputHandler(ChangeEventArgs args)
        {
            //TODO: TRY PARSE
            _text = (string)args.Value;

            DateTime date;

            if (DateTime.TryParse(_text, out date))
            {
                Value = date;
            }

            StateHasChanged();

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

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-input-datepicker")
                 .If("giz-input-datepicker--full-width", () => IsFullWidth)
                 .If("giz-input-datepicker--dialog", () => Variant == PickerVariants.Dialog)
                 .If("giz-input-datepicker--popup", () => Variant == PickerVariants.Inline)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-input-datepicker__dropdown")
                 .If("giz-input-datepicker__dropdown--cursor", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If("giz-datepicker-dropdown-full-width", () => OpenDirection != PopupOpenDirections.Cursor)
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