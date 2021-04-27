using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

        private string _text;

        #region PROPERTIES

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string LeftIcon { get; set; }

        [Parameter]
        public string RightIcon { get; set; }

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
                 .Add("g-input")
                 .AsString();

        protected string FieldClassName => new ClassMapper()
                 .Add("g-input-field")
                 .AsString();

        protected string ValidationClassName => new ClassMapper()
                 .If("valid", () => _isValid)
                 .If("invalid", () => !_isValid)
                 .AsString();
    }
}