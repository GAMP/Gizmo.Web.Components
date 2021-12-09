using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class DatePickerBase<TValue> : GizInputBase<TValue>
    {
        #region CONSTRUCTOR
        public DatePickerBase()
        {
            //Set default culture and format;
            _culture = CultureInfo.CurrentCulture;
            _format = _culture.DateTimeFormat.ShortDatePattern;
            _converter = new DateConverter<TValue>();
            _converter.Culture = _culture;
            _converter.Format = _format;

            CurrentVisibleMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }
        #endregion

        #region FIELDS

        private CultureInfo _culture;
        private string _format;
        private DateConverter<TValue> _converter;
        private DateTime _currentVisibleMonth;
        private int _monthDays = 0;
        private int _whiteSpaces = 0;
        private bool _showMonthPicker;
        private bool _showYearPicker;
        private bool _requiresScrolling;
        private bool _timePickerIsOpen;
        private DateTime? _previousValue;

        #endregion

        #region PROPERTIES

        public DateTime CurrentVisibleMonth
        {
            get
            {
                return _currentVisibleMonth;
            }
            set
            {
                _currentVisibleMonth = value;

                _monthDays = DateTime.DaysInMonth(_currentVisibleMonth.Year, _currentVisibleMonth.Month);
                _whiteSpaces = (int)_currentVisibleMonth.DayOfWeek;
            }
        }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public bool ShowTime { get; set; }

        [Parameter]
        public CultureInfo Culture { get; set; }

        [Parameter]
        public string Format { get; set; }

        #endregion

        #region METHODS

        private bool IsCurrentDay(int year, int month, int day)
        {
            DateTime? value = _converter.SetValue(Value);

            if (value.HasValue)
            {
                if (value.Value.Year == year &&
                    value.Value.Month == month &&
                    value.Value.Day == day)
                    return true;
            }

            return false;
        }

        private async Task ScrollDatePickerYearIntoView()
        {
            await InvokeVoidAsync("scrollDatePickerYear");
        }

        #endregion

        #region EVENTS

        private async Task TimePickerValueChanged(TValue value)
        {
            _timePickerIsOpen = false;

            await SetValueAsync(value);
        }

        private Task OnClickButtonYearHandler(MouseEventArgs args)
        {
            _showMonthPicker = false;
            _showYearPicker = true;

            _requiresScrolling = true;

            return Task.CompletedTask;
        }

        private Task OnClickButtonMonthHandler(MouseEventArgs args)
        {
            _showMonthPicker = true;

            return Task.CompletedTask;
        }

        private Task OnClickButtonPreviousMonthHandler(MouseEventArgs args)
        {
            CurrentVisibleMonth = CurrentVisibleMonth.AddMonths(-1);

            return Task.CompletedTask;
        }

        private Task OnClickButtonNextMonthHandler(MouseEventArgs args)
        {
            CurrentVisibleMonth = CurrentVisibleMonth.AddMonths(1);

            return Task.CompletedTask;
        }

        private Task OnClickButtonPreviousYearHandler(MouseEventArgs args)
        {
            CurrentVisibleMonth = CurrentVisibleMonth.AddYears(-1);

            return Task.CompletedTask;
        }

        private Task OnClickButtonNextYearHandler(MouseEventArgs args)
        {
            CurrentVisibleMonth = CurrentVisibleMonth.AddYears(1);

            return Task.CompletedTask;
        }

        private async Task OnClickButtonDay(int day)
        {
            TValue newValue;

            DateTime? oldValue = _converter.SetValue(Value);
            if (oldValue.HasValue)
            {
                newValue = _converter.GetValue(new DateTime(CurrentVisibleMonth.Year, CurrentVisibleMonth.Month, day, oldValue.Value.Hour, oldValue.Value.Minute, oldValue.Value.Second));
            }
            else
            {
                newValue = _converter.GetValue(new DateTime(CurrentVisibleMonth.Year, CurrentVisibleMonth.Month, day));
            }

            await SetValueAsync(newValue);
        }

        private Task OnClickButtonMonth(int month)
        {
            CurrentVisibleMonth = new DateTime(CurrentVisibleMonth.Year, month, 1);
            _showMonthPicker = false;

            return Task.CompletedTask;
        }

        private Task OnClickButtonYear(int year)
        {
            CurrentVisibleMonth = new DateTime(year, CurrentVisibleMonth.Month, 1);
            _showYearPicker = false;
            _showMonthPicker = true;

            return Task.CompletedTask;
        }

        private void OnClickTimePickerHandler(MouseEventArgs args)
        {
            _timePickerIsOpen = true;
        }

        private void OnClickTimePickerOK()
        {
            _timePickerIsOpen = false;
        }

        private void OnClickTimePickerCancel()
        {
            _timePickerIsOpen = false;
        }

        #endregion

        #region METHODS

        protected async Task SetValueAsync(TValue value)
        {
            Value = value;

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
                CurrentVisibleMonth = new DateTime(value.Value.Year, value.Value.Month, 1);
            }

            await base.OnFirstAfterRenderAsync();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender && _requiresScrolling)
            {
                await ScrollDatePickerYearIntoView();
                _requiresScrolling = false;
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            DateTime? value = _converter.SetValue(Value);

            bool newValue = !EqualityComparer<DateTime?>.Default.Equals(_previousValue, value);
            _previousValue = value;

            if (newValue)
            {
                if (value.HasValue)
                    CurrentVisibleMonth = new DateTime(value.Value.Year, value.Value.Month, 1);
                else
                    CurrentVisibleMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
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
                if (ShowTime)
                {
                    _format = _culture.DateTimeFormat.ShortDatePattern + " " + _culture.DateTimeFormat.ShortTimePattern;
                }
                else
                {
                    _format = _culture.DateTimeFormat.ShortDatePattern;
                }
            }

            _converter.Culture = _culture;
            _converter.Format = _format;

            await base.OnParametersSetAsync();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .AsString();

        #endregion

    }
}