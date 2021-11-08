using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class TimePickerBase : GizInputBase<DateTime?>
    {
        #region FIELDS

        private DateTime? _previewValue;
        private DateTime? _value;
        private int _hours;
        private int _minutes;
        private bool _am = true;

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

                ReloadValue();

                ValueChanged.InvokeAsync(Value);
            }
        }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public EventCallback OnClickOK { get; set; }

        [Parameter]
        public EventCallback OnClickCancel { get; set; }

        #endregion

        #region METHODS

        internal void ReloadValue()
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

        protected async Task OnClickOKButtonHandler(MouseEventArgs args)
        {
            Value = _previewValue;

            await OnClickOK.InvokeAsync();
        }

        protected async Task OnClickCancelButtonHandler(MouseEventArgs args)
        {
            ReloadValue();

            await OnClickCancel.InvokeAsync();
        }

        #endregion
    }
}