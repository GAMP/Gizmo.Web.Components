using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class TextInput : InputBase<string>
    {
        public enum TextInputSize
        {
            Normal = 0,
            Large = 1
        }

        #region CONSTRUCTOR
        public TextInput()
        {
        }
        #endregion

        private string _text;

        #region PROPERTIES

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string LeftIcon { get; set; }

        [Parameter]
        public string RightIcon { get; set; }

        [Parameter]
        public TextInputSize Size { get; set; } = TextInputSize.Normal;

        [Parameter]
        public bool HasOutline { get; set; } = true;

        [Parameter]
        public bool HasShadow { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

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

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var valueChanged = parameters.TryGetValue<string>(nameof(Value), out var newValue);
            if (valueChanged)
            {
                _text = Value;
            }
        }

        protected string ClassName => new ClassMapper()
                 .Add("giz-input-control")
                 .AsString();

        protected string IconLeft => new ClassMapper()
                .Add("giz-input-icon-left")
                .AsString();

        protected string IconRight => new ClassMapper()
                .Add("giz-input-icon-right")
                .AsString();

        protected string FieldClassName => new ClassMapper()                 
                 .Add("giz-input-root")
                 .If("giz-input-root--disabled", () => IsDisabled)
                 .If("giz-input-root--outline", () => HasOutline)
                 .If("giz-input-root--shadow", () => HasShadow)
                 .If("giz-input-root--full-width", () => IsFullWidth)
                 .If("giz-input-root--large", () => Size== TextInputSize.Large)
                 .AsString();

        protected string ValidationClassName => new ClassMapper()
                 .If("giz-input-valid", () => _isValid)
                 .If("giz-input-invalid", () => !_isValid)
                 .AsString();
    }
}