using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;
using static Gizmo.Web.Components.GizInput;

namespace Gizmo.Web.Components
{
    public partial class DatePicker : InputBase<DateTime?>
    {
        #region CONSTRUCTOR
        public DatePicker()
        {
        }
        #endregion

        #region MEMBERS
        private DateTime? _value;
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
        public int MaximumHeight { get; set; }

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

        #endregion

        #region EVENTS

        public void OnInputHandler(ChangeEventArgs args)
        {
            //TODO: TRY PARSE
            _text = (string)args.Value;

            StateHasChanged();
        }

        protected Task OnInputClickHandler(MouseEventArgs args)
        {
            return Task.CompletedTask;
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
                 .Add("giz-input-datepicker")
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-input-datepicker-dropdown-menu")
                 .Add("giz-datepicker-dropdown-full-width")
                 .Add("g-popup-bottom")
                 .AsString();

    }
}