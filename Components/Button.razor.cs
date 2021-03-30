using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Button: CustomDOMComponentBase
    {
        public Button()
        {
            ClassMapper
                .Add("custom-button")
                .If("style-disabled",()=> IsDisabled);
        }

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

        [Parameter()]
        public string Label { get; set; }

        /// <summary>
        /// Inline label of Button.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        #region DOM EVENT HANDLERS

        protected async Task OnClickHandler(MouseEventArgs args)
        {
            IsDisabled = true;

            await Task.Delay(1000);
            IsDisabled = false;

            
        }

        #endregion
    }
}
