using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
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
                if (FirstDayOfWeek == DayOfWeek.Sunday)
                {
                    _whiteSpaces = (int)_currentVisibleMonth.DayOfWeek;
                }
                else
                {
                    _whiteSpaces = (int)_currentVisibleMonth.DayOfWeek - (int)FirstDayOfWeek;
                    if (_whiteSpaces < 0)
                    {
                        _whiteSpaces = 7 + _whiteSpaces;
                    }
                }
            }
        }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public TValue Range { get; set; }

        [Parameter]
        public TValue MinValue { get; set; }

        [Parameter]
        public TValue MaxValue { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public bool ShowTime { get; set; }

        [Parameter]
        public bool CanSwitchToMonthAndYear { get; set; }

        [Parameter]
        public CultureInfo Culture { get; set; }

        [Parameter]
        public string Format { get; set; }

        [Parameter]
        public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Sunday;

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

        private void OnClickButtonYearHandler(MouseEventArgs args)
        {
            _showMonthPicker = false;
            _showYearPicker = true;

            _requiresScrolling = true;
        }

        private void OnClickButtonMonthHandler(MouseEventArgs args)
        {
            if (CanSwitchToMonthAndYear)
                _showMonthPicker = true;
        }

        private void OnClickButtonPreviousMonthHandler(MouseEventArgs args)
        {
            CurrentVisibleMonth = CurrentVisibleMonth.AddMonths(-1);
        }

        private void OnClickButtonNextMonthHandler(MouseEventArgs args)
        {
            CurrentVisibleMonth = CurrentVisibleMonth.AddMonths(1);
        }

        private void OnClickButtonPreviousYearHandler(MouseEventArgs args)
        {
            CurrentVisibleMonth = CurrentVisibleMonth.AddYears(-1);
        }

        private void OnClickButtonNextYearHandler(MouseEventArgs args)
        {
            CurrentVisibleMonth = CurrentVisibleMonth.AddYears(1);
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

        private void OnClickButtonMonth(int month)
        {
            CurrentVisibleMonth = new DateTime(CurrentVisibleMonth.Year, month, 1);
            _showMonthPicker = false;
        }

        private void OnClickButtonYear(int year)
        {
            CurrentVisibleMonth = new DateTime(year, CurrentVisibleMonth.Month, 1);
            _showYearPicker = false;
            _showMonthPicker = true;
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

        private string GetDayClass(int day)
        {
            var result = "giz-date-picker-day-button";

            if (IsCurrentDay(CurrentVisibleMonth.Year, CurrentVisibleMonth.Month, day))
            {
                result += " active";

                if (Range != null)
                {
                    DateTime? range = _converter.SetValue(Range);
                    DateTime? value = _converter.SetValue(Value);

                    if (range > value)
                        result += " range-right";
                    else
                        result += " range-left";
                }
            }

            if (Range != null)
            {
                DateTime? range = _converter.SetValue(Range);
                DateTime? value = _converter.SetValue(Value);
                DateTime currentDay = new DateTime(CurrentVisibleMonth.Year, CurrentVisibleMonth.Month, day);

                if (range > value)
                {
                    if (currentDay > value && currentDay < range)
                        result += " range-right";
                }
                else
                {
                    if (currentDay < value && currentDay > range)
                        result += " range-left";
                }
            }

            return result;
        }

        private bool GetDayDisabled(int day)
        {
            DateTime currentDay = new DateTime(CurrentVisibleMonth.Year, CurrentVisibleMonth.Month, day);

            if (MinValue != null)
            {
                DateTime? minValue = _converter.SetValue(MinValue);

                if (currentDay < minValue)
                {
                    return true;
                }
            }

            if (MaxValue != null)
            {
                DateTime? maxValue = _converter.SetValue(MaxValue);

                if (currentDay > maxValue)
                {
                    return true;
                }
            }

            return false;
        }

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
                 .Add("giz-date-picker")
                 .If("disabled", () => IsDisabled)
                 .AsString();

        #endregion
    }
}
