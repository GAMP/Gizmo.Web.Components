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
            Outline
        }

        #region CONSTRUCTOR
        public Button()
        {
        }
        #endregion

        #region PRIVATE FIELDS
        private bool _isSelected;
        #endregion

        #region PROPERTIES

        #region PUBLIC

        [CascadingParameter]
        protected ButtonGroup ButtonGroup { get; set; }

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

        /// <summary>
        /// Gets or sets element type.
        /// </summary>
        [Parameter()]
        public string Type { get; set; }

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
        public bool Selected { get; set; }

        #endregion

        #endregion

        #region DOM EVENT HANDLERS

        protected Task OnClickHandler(MouseEventArgs args)
        {
            if (ButtonGroup != null)
            {
                ButtonGroup.SetSelectedItem(this);
            }

            return Task.CompletedTask;
        }

        #endregion

        protected string ClassName => new ClassMapper()
                 .Add("button")
                 .Add($"button--{Size.ToDescriptionString()}")
                 .If("button--secondary", () => ButtonGroup == null && Variant == ButtonVariants.Outline)
                 .If("disabled", () => IsDisabled)
                 .If("selected", () => _isSelected)
                 .AsString();

        protected override void OnInitialized()
        {
            _isSelected = Selected;

            if (ButtonGroup != null)
            {
                ButtonGroup.Register(this);

                if (_isSelected)
                {
                    ButtonGroup.SetSelectedItem(this);
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

            if (_isSelected == selected)
                return;

            _isSelected = selected;

            StateHasChanged();
        }
    }
}