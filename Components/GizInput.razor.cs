﻿using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class GizInput : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public GizInput()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public ValidationErrorStyles ValidationErrorStyle { get; set; } = ValidationErrorStyles.Label;

        /// <summary>
        /// Gets or sets if input is disabled.
        /// </summary>
        [Parameter]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Gets or sets if input is read only.
        /// </summary>
        [Parameter]
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets if input is hidden.
        /// </summary>
        [Parameter]
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets or sets the label of the input.
        /// </summary>
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public RenderFragment LeftContent { get; set; }

        [Parameter]
        public RenderFragment RightContent { get; set; }

        [Parameter]
        public string LeftIcon { get; set; }

        [Parameter]
        public string RightIcon { get; set; }

        [Parameter]
        public Icons? LeftSVGIcon { get; set; }

        [Parameter]
        public Icons? RightSVGIcon { get; set; }

        /// <summary>
        /// Gets or sets the size of the input.
        /// </summary>
        [Parameter]
        public InputSizes Size { get; set; } = InputSizes.Medium;

        /// <summary>
        /// Gets or sets if input has outline.
        /// </summary>
        [Parameter]
        public bool HasOutline { get; set; } = true;

        /// <summary>
        /// Gets or sets if input has shadow.
        /// </summary>
        [Parameter]
        public bool HasShadow { get; set; }

        /// <summary>
        /// Gets or sets if input is transparent.
        /// </summary>
        [Parameter]
        public bool IsTransparent { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        /// <summary>
        /// Gets or sets the width of the input.
        /// </summary>
        [Parameter]
        public string Width { get; set; } = "20rem";

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>
        /// Gets or sets the content of the input.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsValid { get; set; }

        [Parameter]
        public string ValidationMessage { get; set; }

        #endregion

        #region EVENTS

        protected Task OnChangeHandler(ChangeEventArgs args)
        {
            return Task.CompletedTask;
        }

        protected Task OnClickHandler(MouseEventArgs args)
        {
            return OnClick.InvokeAsync(args);
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-input-control")
                 .If("giz-input-control--valid", () => IsValid)
                 .If("giz-input-control--invalid", () => !IsValid)
                 .AsString();

        protected string IconLeft => new ClassMapper()
                .Add("giz-input-icon-left")
                .AsString();

        protected string IconRight => new ClassMapper()
                .Add("giz-input-icon-right")
                .AsString();

        protected string FieldClassName => new ClassMapper()
                 .Add("giz-input-root")
                 .Add($"giz-input-root--{Size.ToDescriptionString()}")
                 .If("giz-input-root--outline", () => HasOutline)
                 .If("giz-input-root--shadow", () => HasShadow)
                 .If("giz-input-root--filled", () => !IsTransparent)
                 .If("giz-input-root--transparent", () => IsTransparent)
                 .If("giz-input-root--full-width", () => IsFullWidth)
                 .AsString();

        protected string FieldStyleValue => new StyleMapper()
                 .If($"width: {Width}", () => !IsFullWidth)
                 .AsString();

        protected string ValidationClassName => new ClassMapper()
                 .If("giz-input-validation-label", () => ValidationErrorStyle == ValidationErrorStyles.Label)
                 .If("giz-input-validation-tooltip", () => ValidationErrorStyle == ValidationErrorStyles.Tooltip)
                 .AsString();

        #endregion

    }
}