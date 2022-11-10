using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class PasswordInput : GizInputBase<string>
    {
        #region CONSTRUCTOR
        public PasswordInput()
        {
        }
        #endregion

        #region FIELDS

        private string _text;
        private bool _showPassword;

        #endregion

        #region PROPERTIES

        [Parameter]
        public ValidationErrorStyles ValidationErrorStyle { get; set; } = ValidationErrorStyles.Label;

        [Parameter]
        public string Label { get; set; }

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
        public string Width { get; set; } = "20rem";

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public int MaxLength { get; set; }

        [Parameter]
        public bool UpdateOnInput { get; set; }

        public bool IsValid => _isValid;

        public string ValidationMessage => _validationMessage;

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
            await ValueChanged.InvokeAsync(Value);
            NotifyFieldChanged();
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
            if (parameters.TryGetValue<string>(nameof(Value), out var newValue))
            {
                var valueChanged = Value != newValue;
                if (valueChanged)
                {
                    _text = Value;
                }
            }

            await base.SetParametersAsync(parameters);
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