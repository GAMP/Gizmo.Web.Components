using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class PasswordInput : GizInputBase<string>, IGizInput
    {
        #region CONSTRUCTOR
        public PasswordInput()
        {
        }
        #endregion

        #region FIELDS

        private string _text;
        private bool _showPassword;

        private string _previousValue;

        #endregion

        #region PROPERTIES

        #region IGizInput

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public string LeftIcon { get; set; }

        [Parameter]
        public string RightIcon { get; set; }

        [Parameter]
        public Icons? LeftSVGIcon { get; set; }

        [Parameter]
        public Icons? RightSVGIcon { get; set; }

        [Parameter]
        public InputSizes Size { get; set; } = InputSizes.Medium;

        [Parameter]
        public bool HasOutline { get; set; } = true;

        [Parameter]
        public bool HasShadow { get; set; }

        [Parameter]
        public bool IsTransparent { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public string Width { get; set; }

        [Parameter]
        public ValidationErrorStyles ValidationErrorStyle { get; set; } = ValidationErrorStyles.Label;

        public bool IsValid => _isValid;

        public string ValidationMessage => _validationMessage;

        #endregion

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public int MaxLength { get; set; }

        [Parameter]
        public bool UpdateOnInput { get; set; }

        #endregion

        #region EVENTS

        protected Task OnInputHandler(ChangeEventArgs args)
        {
            if (UpdateOnInput)
            {
                var newText = args?.Value as string;

                string newValue = newText;

                if (!EqualityComparer<string>.Default.Equals(Value, newValue))
                {
                    return SetValueAsync(newValue);
                }
            }

            return Task.CompletedTask;
        }

        protected Task OnChangeHandler(ChangeEventArgs args)
        {
            if (!UpdateOnInput)
            {
                var newText = args?.Value as string;

                string newValue = newText;

                if (!EqualityComparer<string>.Default.Equals(Value, newValue))
                {
                    return SetValueAsync(newValue);
                }
            }

            return Task.CompletedTask;
        }

        protected Task OnClickHandler(MouseEventArgs args)
        {
            return OnClick.InvokeAsync(args);
        }

        public void OnClickButtonEyeHandler(MouseEventArgs args)
        {
            _showPassword = !_showPassword;
        }

        #endregion

        #region METHODS

        protected async Task SetValueAsync(string value)
        {
            Value = value;
            UpdateText();
            await ValueChanged.InvokeAsync(Value);
            NotifyFieldChanged();
        }

        private void UpdateText()
        {
            var valueChanged = _previousValue != Value;
            if (valueChanged)
            {
                _previousValue = Value;

                _text = Value;
            }
        }

        #endregion

        #region OVERRIDES

        protected override Task OnFirstAfterRenderAsync()
        {
            var attributes = new Dictionary<string, object>();

            if (MaxLength > 0)
                attributes["maxlength"] = MaxLength;

            Attributes = attributes;

            return base.OnFirstAfterRenderAsync();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            UpdateText();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-input-password")
                 .If("giz-input-password--full-width", () => IsFullWidth)
                 .Add(Class)
                 .AsString();

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}