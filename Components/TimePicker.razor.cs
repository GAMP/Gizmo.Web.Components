using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class TimePicker : GizInputBase<DateTime?>
    {
        #region CONSTRUCTOR
        public TimePicker()
        {
        }
        #endregion

        #region FIELDS
        private DateTime? _previewValue;
        private DateTime? _value;
        private int _hours;
        private int _minutes;
        private bool _am = true;
        private string _text;
        #endregion

        #region PROPERTIES

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
                    _text = _value.Value.ToString("hh:mm tt");
                }
                else
                {
                    _text = string.Empty;
                }

                ReloadValue();

                ValueChanged.InvokeAsync(Value);
            }
        }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

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

        #endregion

        #region METHODS

        private void ReloadValue()
        {
            if (Value.HasValue)
            {
                if (Value.Value.Hour < 12)
                {
                    _hours = Value.Value.Hour;
                    _am = true;
                }
                else
                {
                    _hours = Value.Value.Hour - 12;
                    _am = false;
                }

                _minutes = Value.Value.Minute;
            }
            else
            {
                _hours = 0;
                _minutes = 0;
                _am = true;
            }

            _previewValue = new DateTime(1, 1, 1, _am ? _hours : _hours + 12, _minutes, 0);
        }

        #endregion

        #region EVENTS

        private Task OnClickButtonIncreaseHourHandler(MouseEventArgs args)
        {
            if (_hours < 11)
                _hours += 1;
            else
                _hours = 0;

            _previewValue = new DateTime(1, 1, 1, _am ? _hours : _hours + 12, _minutes, 0);

            return Task.CompletedTask;
        }

        private Task OnClickButtonDecreaseHourHandler(MouseEventArgs args)
        {
            if (_hours > 0)
                _hours -= 1;
            else
                _hours = 11;

            _previewValue = new DateTime(1, 1, 1, _am ? _hours : _hours + 12, _minutes, 0);

            return Task.CompletedTask;
        }

        private Task OnClickButtonIncreaseMinuteHandler(MouseEventArgs args)
        {
            if (_minutes < 59)
                _minutes += 1;
            else
                _minutes = 0;

            _previewValue = new DateTime(1, 1, 1, _am ? _hours : _hours + 12, _minutes, 0);

            return Task.CompletedTask;
        }

        private Task OnClickButtonDecreaseMinuteHandler(MouseEventArgs args)
        {
            if (_minutes > 0)
                _minutes -= 1;
            else
                _minutes = 59;

            _previewValue = new DateTime(1, 1, 1, _am ? _hours : _hours + 12, _minutes, 0);

            return Task.CompletedTask;
        }

        private Task OnClickButtonSwitchAMPMHandler(MouseEventArgs args)
        {
            _am = !_am;
            _previewValue = new DateTime(1, 1, 1, _am ? _hours : _hours + 12, _minutes, 0);

            return Task.CompletedTask;
        }

        public Task OnInputHandler(ChangeEventArgs args)
        {
            //TODO: TRY PARSE
            _text = (string)args.Value;

            StateHasChanged();

            return Task.CompletedTask;
        }

        protected Task OnClickInputHandler(MouseEventArgs args)
        {
            IsOpen = true;

            return Task.CompletedTask;
        }

        protected Task OnClickOverlayHandler(MouseEventArgs args)
        {
            ReloadValue();

            IsOpen = false;

            return Task.CompletedTask;
        }

        protected Task OnClickOKButtonHandler(MouseEventArgs args)
        {
            Value = _previewValue;

            IsOpen = false;

            return Task.CompletedTask;
        }

        protected Task OnClickCancelButtonHandler(MouseEventArgs args)
        {
            ReloadValue();

            IsOpen = false;

            return Task.CompletedTask;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-input-timepicker")
                 .Add("giz-input-timepicker--popup")
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-input-timepicker-dropdown-menu")
                 .Add("giz-timepicker-dropdown-full-width")
                 .AsString();

        #endregion

    }
}