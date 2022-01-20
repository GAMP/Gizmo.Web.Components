using Microsoft.AspNetCore.Components;
using System.Windows.Input;

namespace Gizmo.Web.Components
{
    public class ButtonBase : CustomDOMComponentBase
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or sets element type.
        /// </summary>
        [Parameter]
        public string Type { get; set; } = "button";

        /// <summary>
        /// Gets or sets element name.
        /// </summary>
        [Parameter]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets button size.
        /// </summary>
        [Parameter]
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        [Parameter]
        public ButtonColors Color { get; set; } = ButtonColors.Primary;

        /// <summary>
        /// Gets or sets if element is disabled.
        /// </summary>
        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool HasShadow { get; set; }

        [Parameter]
        public ICommand Command { get; set; }

        [Parameter]
        public object CommandParameter { get; set; }

        #endregion
    }
}
