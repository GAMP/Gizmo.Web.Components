using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel;
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
        #endregion

        #region PROPERTIES

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

        #endregion

        #endregion

        #region DOM EVENT HANDLERS

        protected Task OnClickHandler(MouseEventArgs args)
        {
            return Task.CompletedTask;
        }

        #endregion

        protected string ClassName => new ClassMapper()
                 .Add("button")
                 .Add($"button--{Size.ToDescriptionString()}")
                 .If("button--secondary", () => Variant == ButtonVariants.Outline)
                 .If("disabled", () => IsDisabled)
                 .AsString();

    }
}