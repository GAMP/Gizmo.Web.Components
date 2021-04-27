using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Icon : CustomDOMComponentBase
    {
        public enum IconBackgroundStyles
        {
            None,
            Circle,
            Square
        }

        public enum IconSizes
        {
            [Description("xs")]
            Small,

            [Description("1x")]
            Medium,

            [Description("2x")]
            Large
        }

        #region CONSTRUCTOR
        public Icon()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public string Source { get; set; }

        [Parameter]
        public IconSizes Size { get; set; } = IconSizes.Medium;

        [Parameter]
        public string Color { get; set; }

        [Parameter]
        public IconBackgroundStyles BackgroundStyle { get; set; } = IconBackgroundStyles.None;

        [Parameter]
        public string BackgroundColor { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        #endregion

        #region EVENTS

        protected Task OnClickHandler(MouseEventArgs args)
        {
            return OnClick.InvokeAsync(args);
        }

        #endregion

        protected string ClassName => new ClassMapper()
                 .Add($"fa-{Size.ToDescriptionString()}")
                 .If("fa-stack", () => BackgroundStyle != IconBackgroundStyles.None)
                 .AsString();

        protected string IconClassName => new ClassMapper()
                 .If("fa-stack-1x", () => BackgroundStyle != IconBackgroundStyles.None)
                 .AsString();
    }
}