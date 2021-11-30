﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class DatePickerBase : GizInputBase<DateTime?>
    {
        #region CONSTRUCTOR
        public DatePickerBase()
        {
            CurrentVisibleMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }
        #endregion

        #region FIELDS

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
        public DateTime? Value { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public bool ShowTime { get; set; }

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

        private async Task ScrollDatePickerYearIntoView()
        {
            await InvokeVoidAsync("scrollDatePickerYear");
        }

        #endregion

        #region EVENTS

        private async Task TimePickerValueChanged(DateTime? value)
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
            await SetValueAsync(new DateTime(CurrentVisibleMonth.Year, CurrentVisibleMonth.Month, day));
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

        protected async Task SetValueAsync(DateTime? value)
        {
            Value = value;

            await ValueChanged.InvokeAsync(Value);
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnFirstAfterRenderAsync()
        {
            //If the component initialized with a value.
            if (Value.HasValue)
            {
                CurrentVisibleMonth = new DateTime(Value.Value.Year, Value.Value.Month, 1);
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
            bool newValue = !EqualityComparer<DateTime?>.Default.Equals(_previousValue, Value);
            _previousValue = Value;

            if (newValue)
            {
                if (Value.HasValue)
                    CurrentVisibleMonth = new DateTime(Value.Value.Year, Value.Value.Month, 1);
                else
                    CurrentVisibleMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            await base.OnParametersSetAsync();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .AsString();

        #endregion

    }
}