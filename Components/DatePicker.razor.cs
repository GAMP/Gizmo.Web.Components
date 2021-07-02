using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class DatePicker : InputBase<DateTime?>
    {
        #region CONSTRUCTOR
        public DatePicker()
        {
        }
        #endregion

        #region FIELDS
        private DateTime? _value;
        private string _text;
        #endregion

        #region PROPERTIES

        [Parameter]
        public PickerVariants Variant { get; set; } = PickerVariants.Inline;

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
        public bool IsFullWidth { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        #endregion

        #region METHODS

        private Task DatePickerValueChanged(DateTime? value)
        {
            IsOpen = false;
            Value = value;

            return Task.CompletedTask;
        }

        #endregion

        #region EVENTS

        public Task OnInputHandler(ChangeEventArgs args)
        {
            //TODO: TRY PARSE
            _text = (string)args.Value;

            DateTime date;

            if (DateTime.TryParse(_text, out date))
            {
                Value = date;
            }

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
            IsOpen = false;

            return Task.CompletedTask;
        }

        #endregion

        #region OVERRIDE

        protected override async Task OnFirstAfterRenderAsync()
        {
            //If the component initialized with a value.
            if (_value != null)
            {

            }

            await base.OnFirstAfterRenderAsync();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-input-datepicker")
                 .If("giz-input-datepicker--dialog", () => Variant == PickerVariants.Dialog)
                 .If("giz-input-datepicker--popup", () => Variant == PickerVariants.Inline)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-input-datepicker-dropdown-menu")
                 .Add("giz-datepicker-dropdown-full-width")
                 .If("giz-popup-bottom", () => Variant == PickerVariants.Inline)
                 .AsString();

        #endregion

    }
}