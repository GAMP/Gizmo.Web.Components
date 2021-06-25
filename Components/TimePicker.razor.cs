using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;
using static Gizmo.Web.Components.GizInput;

namespace Gizmo.Web.Components
{
    public partial class TimePicker : InputBase<TimeSpan?>
    {
        #region CONSTRUCTOR
        public TimePicker()
        {
        }
        #endregion

        #region MEMBERS
        private TimeSpan? _value;
        private string _text;
        #endregion

        #region PROPERTIES

        [Parameter]
        public TimeSpan? Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                //Update the component's text.
                if (_value != null)
                {
                    _text = _value.ToString();
                }
                else
                {
                    _text = string.Empty;
                }

                StateHasChanged();
            }
        }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public bool OffsetY { get; set; }

        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;

        [Parameter]
        public bool HasOutline { get; set; } = true;

        [Parameter]
        public bool HasShadow { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        #endregion

        #region METHODS

        private void TimePickerValueChanged(TimeSpan? value)
        {
            IsOpen = false;
            Value = value;
        }

        #endregion

        #region EVENTS

        public void OnInputHandler(ChangeEventArgs args)
        {
            //TODO: TRY PARSE
            _text = (string)args.Value;

            StateHasChanged();
        }

        protected void OnMenuClickHandler(MouseEventArgs args)
        {
            IsOpen = true;
        }

        protected void OnOverlayClickHandler(MouseEventArgs args)
        {
            IsOpen = false;
        }

        #endregion

        protected override async Task OnFirstAfterRenderAsync()
        {
            //If the component initialized with a value.
            if (_value != null)
            {

            }

            await base.OnFirstAfterRenderAsync();
        }

        protected string ClassName => new ClassMapper()
                 .Add("giz-input-timepicker")
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-input-timepicker-dropdown-menu")
                 .Add("giz-timepicker-dropdown-full-width")
                 .Add("giz-popup-bottom")
                 .AsString();

    }
}