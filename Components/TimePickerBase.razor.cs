using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class TimePickerBase<TValue> : GizInputBase<TValue>
    {
        #region FIELDS

        private DateConverter<TValue> _converter = new DateConverter<TValue>();
        private DateTime? _previewValue;
        private int _hours;
        private int _minutes;
        private bool _am = true;
        private TValue _previousValue;

        #endregion

        #region PROPERTIES

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public EventCallback OnClickOK { get; set; }

        [Parameter]
        public EventCallback OnClickCancel { get; set; }

        [Parameter]
        public CultureInfo? Culture { get; set; }

        [Parameter]
        public string Format { get; set; }

        #endregion

        #region METHODS

        internal void ReloadValue()
        {
            var value = _converter.SetValue(Value);

            if (value.HasValue)
            {
                if (value.Value.Hour < 12)
                {
                    _hours = value.Value.Hour;
                    _am = true;
                }
                else
                {
                    _hours = value.Value.Hour - 12;
                    _am = false;
                }

                _minutes = value.Value.Minute;
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
            TValue newValue = _converter.GetValue(_previewValue);

            await SetValueAsync(newValue);

            await OnClickOK.InvokeAsync();
        }

        protected async Task OnClickCancelButtonHandler(MouseEventArgs args)
        {
            ReloadValue();

            await OnClickCancel.InvokeAsync();
        }

        #endregion

        #region METHODS

        protected async Task SetValueAsync(TValue value)
        {
            Value = value;

            ReloadValue();

            await ValueChanged.InvokeAsync(Value);
        }

        #endregion

        #region OVERRIDE

        protected override void OnInitialized()
        {
            if (Culture != null && !string.IsNullOrEmpty(Format))
            {
                _converter.Culture = Culture;
                _converter.Format = Format;
            }

            base.OnInitialized();
        }

        protected override async Task OnParametersSetAsync()
        {
            bool newValue = !EqualityComparer<TValue>.Default.Equals(_previousValue, Value);
            _previousValue = Value;

            if (newValue)
            {
                _previewValue = _converter.SetValue(Value);
            }

            await base.OnParametersSetAsync();
        }

        #endregion

    }
}