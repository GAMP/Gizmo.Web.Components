﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class TextInput<TValue> : GizInputBase<TValue>
    {
        #region CONSTRUCTOR
        public TextInput()
        {
        }
        #endregion

        #region FIELDS

        private StringConverter<TValue> _converter = new StringConverter<TValue>();
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
        public Icons? LeftSVGIcon { get; set; }

        [Parameter]
        public Icons? RightSVGIcon { get; set; }

        [Parameter]
        public InputSizes Size { get; set; } = InputSizes.Normal;

        [Parameter]
        public bool HasOutline { get; set; } = true;

        [Parameter]
        public bool HasShadow { get; set; }

        [Parameter]
        public bool IsTransparent { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public TValue Value { get; set; }

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

        [Parameter]
        public bool UpdateOnInput { get; set; }

        [Parameter]
        public bool IsMultiLine { get; set; }

        [Parameter]
        public CultureInfo? Culture { get; set; }

        public bool IsValid => _isValid && !_converter.HasGetError;

        public string ValidationMessage => _converter.HasGetError ? _converter.GetErrorMessage : _validationMessage;

        #endregion

        #region EVENTS

        protected Task OnInputHandler(ChangeEventArgs args)
        {
            if (UpdateOnInput)
            {
                var newText = args?.Value as string;

                TValue newValue = _converter.GetValue(newText);

                if (!EqualityComparer<TValue>.Default.Equals(Value, newValue))
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

                TValue newValue = _converter.GetValue(newText);

                if (!EqualityComparer<TValue>.Default.Equals(Value, newValue))
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

        #endregion

        #region METHODS

        protected async Task SetValueAsync(TValue value)
        {
            Value = value;
            await ValueChanged.InvokeAsync(Value);
        }

        #endregion

        #region OVERRIDE

        protected override void OnInitialized()
        {
            if (Culture != null)
            {
                _converter.Culture = Culture;
            }

            base.OnInitialized();
        }

        protected override Task OnFirstAfterRenderAsync()
        {
            var attributes = new Dictionary<string, object>();

            if (Min > 0)
                attributes["min"] = Min;

            if (Max > 0)
                attributes["max"] = Max;

            if (MaxLength > 0)
                attributes["maxlength"] = MaxLength;

            Attributes = attributes;

            return base.OnFirstAfterRenderAsync();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var valueChanged = parameters.TryGetValue<TValue>(nameof(Value), out var newValue);
            if (valueChanged)
            {
                _text = _converter.SetValue(Value);
            }
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-input-text")
                 .If("giz-input-text--full-width", () => IsFullWidth)
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