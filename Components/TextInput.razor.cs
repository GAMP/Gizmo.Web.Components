using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class TextInput : InputBase<string>
    {
        #region CONSTRUCTOR
        public TextInput()
        {
        }
        #endregion

        #region FIELDS

        private string _text;

        #endregion

        #region PROPERTIES

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string LeftIcon { get; set; }

        [Parameter]
        public string RightIcon { get; set; }

        [Parameter]
        public InputSizes Size { get; set; } = InputSizes.Normal;

        [Parameter]
        public bool HasOutline { get; set; } = true;

        [Parameter]
        public bool HasShadow { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public string Type { get; set; } = "text";

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public int Min { get; set; }

        [Parameter]
        public int Max { get; set; }

        [Parameter]
        public int MaxLength { get; set; }

        #endregion

        #region EVENTS

        protected Task OnChangeHandler(ChangeEventArgs args)
        {
            var newValue = args?.Value as string;

            if (Value != newValue)
            {
                return SetValueAsync(newValue);
            }

            return Task.CompletedTask;
        }

        protected Task OnClickHandler(MouseEventArgs args)
        {
            return OnClick.InvokeAsync(args);
        }

        #endregion

        #region METHODS

        protected async Task SetValueAsync(string value)
        {
            Value = value;
            await ValueChanged.InvokeAsync(Value);
        }

        #endregion

        #region OVERRIDE

        protected override Task OnFirstAfterRenderAsync()
        {
            Attributes = new Dictionary<string, object>();

            if (Min > 0)
                Attributes["min"] = Min;

            if (Max > 0)
                Attributes["max"] = Max;

            if (MaxLength > 0)
                Attributes["maxlength"] = MaxLength;

            return base.OnFirstAfterRenderAsync();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var valueChanged = parameters.TryGetValue<string>(nameof(Value), out var newValue);
            if (valueChanged)
            {
                _text = Value;
            }
        }

        #endregion

    }
}