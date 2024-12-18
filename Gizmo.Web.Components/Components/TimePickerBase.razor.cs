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
        #region CONSTRUCTOR
        public TimePickerBase()
        {
            //Set default culture and format;
            _culture = CultureInfo.CurrentCulture;
            _format = _culture.DateTimeFormat.ShortDatePattern;
            _converter = new DateConverter<TValue>();
            _converter.Culture = _culture;
            _converter.Format = _format;
        }

        #endregion
        #region FIELDS

        private CultureInfo _culture;
        private string _format;
        private DateConverter<TValue> _converter;
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
        public CultureInfo Culture { get; set; }

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

                _previewValue = value;
            }
            else
            {
                _hours = 0;
                _minutes = 0;
                _am = true;

                _previewValue = null;
            }

            SetPreviewValue(_am ? _hours : _hours + 12, _minutes);
        }

        #endregion

        #region EVENTS

        private Task OnClickButtonIncreaseHourHandler(MouseEventArgs args)
        {
            if (_hours < 11)
                _hours += 1;
            else
                _hours = 0;

            SetPreviewValue(_am ? _hours : _hours + 12, _minutes);

            return Task.CompletedTask;
        }

        private Task OnClickButtonDecreaseHourHandler(MouseEventArgs args)
        {
            if (_hours > 0)
                _hours -= 1;
            else
                _hours = 11;

            SetPreviewValue(_am ? _hours : _hours + 12, _minutes);

            return Task.CompletedTask;
        }

        private Task OnClickButtonIncreaseMinuteHandler(MouseEventArgs args)
        {
            if (_minutes < 59)
                _minutes += 1;
            else
                _minutes = 0;

            SetPreviewValue(_am ? _hours : _hours + 12, _minutes);

            return Task.CompletedTask;
        }

        private Task OnClickButtonDecreaseMinuteHandler(MouseEventArgs args)
        {
            if (_minutes > 0)
                _minutes -= 1;
            else
                _minutes = 59;

            SetPreviewValue(_am ? _hours : _hours + 12, _minutes);

            return Task.CompletedTask;
        }

        private Task OnClickButtonSwitchAMPMHandler(MouseEventArgs args)
        {
            _am = !_am;

            SetPreviewValue(_am ? _hours : _hours + 12, _minutes);

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

        private void SetPreviewValue(int hours, int minutes)
        {
            if (_previewValue == null)
                _previewValue = new DateTime(1, 1, 1, _am ? _hours : _hours + 12, _minutes, 0);
            else
                _previewValue = new DateTime(_previewValue.Value.Year, _previewValue.Value.Month, _previewValue.Value.Day, hours, minutes, 0);
        }

        protected async Task SetValueAsync(TValue value)
        {
            Value = value;

            ReloadValue();

            await ValueChanged.InvokeAsync(Value);
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnFirstAfterRenderAsync()
        {
            //If the component initialized with a value.
            DateTime? value = _converter.SetValue(Value);

            if (value.HasValue)
            {
                ReloadValue();
            }

            await base.OnFirstAfterRenderAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            bool newValue = !EqualityComparer<TValue>.Default.Equals(_previousValue, Value);
            _previousValue = Value;

            if (newValue)
            {
                ReloadValue();
            }

            if (Culture != null)
            {
                _culture = Culture;
            }
            else
            {
                _culture = CultureInfo.CurrentCulture;
            }

            if (!string.IsNullOrEmpty(Format))
            {
                _format = Format;
            }
            else
            {
                _format = _culture.DateTimeFormat.ShortTimePattern;
            }

            _converter.Culture = _culture;
            _converter.Format = _format;

            await base.OnParametersSetAsync();
        }

        #endregion

    }
}