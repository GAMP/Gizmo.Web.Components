using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class DatePickerBase : InputBase<DateTime?>
    {
        #region CONSTRUCTOR
        public DatePickerBase()
        {
            CurrentVisibleMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }
        #endregion

        #region MEMBERS
        private DateTime _currentVisibleMonth;
        private int _monthDays = 0;
        private int _whiteSpaces = 0;
        private DateTime? _value;
        private bool _showMonthPicker;
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
        public DateTime? Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;

                    if (_value.HasValue)
                        CurrentVisibleMonth = new DateTime(_value.Value.Year, _value.Value.Month, 1);
                    else
                        CurrentVisibleMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                    ValueChanged.InvokeAsync(_value);
                }
            }
        }

        [Parameter]
        public bool IsFullWidth { get; set; }

        #endregion

        #region METHODS

        private bool IsCurrentDay(int year, int month, int day)
        {
            if (Value.HasValue)
            {
                if (Value.Value.Year == year &&
                    Value.Value.Month == month &&
                    Value.Value.Day == day)
                    return true;
            }

            return false;
        }

        #endregion

        #region EVENTS

        private void OnClickYearHandler(MouseEventArgs args)
        {
            //TODO: Switch to year picker.
        }

        private void OnClickMonthHandler(MouseEventArgs args)
        {
            _showMonthPicker = true;
        }

        private void OnClickPreviousMonthHandler(MouseEventArgs args)
        {
            CurrentVisibleMonth = CurrentVisibleMonth.AddMonths(-1);
        }

        private void OnClickNextMonthHandler(MouseEventArgs args)
        {
            CurrentVisibleMonth = CurrentVisibleMonth.AddMonths(1);
        }

        private void OnClickDay(int day)
        {
            Value = new DateTime(CurrentVisibleMonth.Year, CurrentVisibleMonth.Month, day);
        }

        private void OnClickMonth(int month)
        {
            CurrentVisibleMonth = new DateTime(CurrentVisibleMonth.Year, month, 1);
            _showMonthPicker = false;
        }

        #endregion

        protected override async Task OnFirstAfterRenderAsync()
        {
            //If the component initialized with a value.
            if (_value.HasValue)
            {
                CurrentVisibleMonth = new DateTime(_value.Value.Year, _value.Value.Month, 1);
            }

            await base.OnFirstAfterRenderAsync();
        }

        protected string ClassName => new ClassMapper()
                 .AsString();

    }
}