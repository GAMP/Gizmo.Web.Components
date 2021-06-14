using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Button : CustomDOMComponentBase
    {
        public enum ButtonVariants
        {
            Fill,
            Outline,
            Text,
            Icon
        }

        #region CONSTRUCTOR
        public Button()
        {
        }
        #endregion

        #region PRIVATE FIELDS
        private bool _selected;
        #endregion

        #region PROPERTIES

        [CascadingParameter]
        protected ButtonGroup ButtonGroup { get; set; }

        #region PUBLIC

        [Parameter()]
        public ButtonVariants Variant { get; set; } = ButtonVariants.Fill;

        /// <summary>
        /// Gets or sets button size.
        /// </summary>
        [Parameter()]
        public ButtonSize Size { get; set; } = ButtonSize.Medium;

        /// <summary>
        /// Gets or sets element name.
        /// </summary>
        [Parameter()]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets if element is disabled.
        /// </summary>
        [Parameter()]
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        /// <summary>
        /// Gets or sets element type.
        /// </summary>
        [Parameter()]
        public string Type { get; set; } = "button";

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        [Parameter()]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets label.
        /// </summary>
        [Parameter()]
        public string Label { get; set; }

        /// <summary>
        /// Inline label of Button.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string LeftIcon { get; set; }

        [Parameter]
        public string RightIcon { get; set; }

        [Parameter]
        public bool IsSelected { get; set; }

        #endregion

        #endregion

        #region DOM EVENT HANDLERS

        protected Task OnClickHandler(MouseEventArgs args)
        {
            if (ButtonGroup != null)
            {
                ButtonGroup.SelectItem(this, !_selected);
            }

            return Task.CompletedTask;
        }

        #endregion

        protected string ButtonIconLeft => new ClassMapper()
                .Add("giz-button-icon-left")
                .AsString();

        protected string ButtonIconRight => new ClassMapper()
                .Add("giz-button-icon-right")
                .AsString();

        protected string ClassName => new ClassMapper()
                 .Add("giz-button")
                 .Add($"giz-button--{Size.ToDescriptionString()}")
                 .If("giz-button--secondary-outline", () => ButtonGroup == null && Variant == ButtonVariants.Outline)
                 .If("giz-button--text", () => ButtonGroup == null && Variant == ButtonVariants.Text)
                 .If("giz-button-full-width", () => IsFullWidth)
                 .If("disabled", () => IsDisabled)
                 .If("selected", () => _selected)
                 .AsString();

        protected override void OnInitialized()
        {
            _selected = IsSelected;

            if (ButtonGroup != null)
            {
                ButtonGroup.Register(this);

                if (_selected)
                {
                    ButtonGroup.SelectItem(this, true);
                }
            }
        }

        public override void Dispose()
        {
            try
            {
                if (ButtonGroup != null)
                {
                    ButtonGroup.Unregister(this);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }

        internal void SetSelected(bool selected)
        {
            if (IsDisabled)
                return;

            if (_selected == selected)
                return;

            _selected = selected;
            IsSelected = _selected;

            StateHasChanged();
        }

        internal bool GetSelected()
        {
            return _selected;
        }
    }
}