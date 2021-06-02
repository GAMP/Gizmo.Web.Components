﻿using Microsoft.AspNetCore.Components;
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
                 .Add("giz-input")
                 .AsString();

        protected string FieldClassName => new ClassMapper()
                 .Add("giz-input-field")
                 .If("giz-input-outline", () => HasOutline)
                 .If("giz-input-shadow", () => HasShadow)
                 .If("giz-input-full-width", () => IsFullWidth)
                 .If("giz-input-large", () => Size== TextInputSize.Large)
                 .AsString();

        protected string ValidationClassName => new ClassMapper()
                 .If("valid", () => _isValid)
                 .If("invalid", () => !_isValid)
                 .AsString();
    }
}