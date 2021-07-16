using Gizmo.Web.Components.Extensions;
using Gizmo.Web.Components.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Icon : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Icon()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public string Source { get; set; }

        [Parameter]
        public Icons? SVGIcon { get; set; }

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

        public decimal Width
        {
            get
            {
                switch (Size)
                {
                    case IconSizes.Small:
                        return 1.6m;

                    case IconSizes.Medium:
                        return 2.2m;

                    case IconSizes.Large:
                        return 3.4m;

                    default:
                        return 2.2m;
                }
            }
        }

        public decimal Height
        {
            get
            {
                switch (Size)
                {
                    case IconSizes.Small:
                        return 1.6m;

                    case IconSizes.Medium:
                        return 2.2m;

                    case IconSizes.Large:
                        return 3.4m;

                    default:
                        return 2.2m;
                }
            }
        }

        #endregion

        #region EVENTS

        protected Task OnClickHandler(MouseEventArgs args)
        {
            return OnClick.InvokeAsync(args);
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add($"fa-{Size.ToDescriptionString()}")
                 .If("fa-stack", () => BackgroundStyle != IconBackgroundStyles.None)
                 .AsString();

        protected string IconClassName => new ClassMapper()
                 .If("fa-stack-1x", () => BackgroundStyle != IconBackgroundStyles.None)
                 .AsString();

        protected string StyleValue => new StyleMapper()
                 .Add($"padding: 0.2em")
                 .Add($"display: inline-block")
                 .Add($"width: { Width.ToString(System.Globalization.CultureInfo.InvariantCulture) }em")
                 .Add($"height: { Height.ToString(System.Globalization.CultureInfo.InvariantCulture) }em")
                 .Add($"border-radius: { (BackgroundStyle == IconBackgroundStyles.Circle ? "50%" : "0") }")
                 .If($"background-color: { BackgroundColor }", () => BackgroundStyle != IconBackgroundStyles.None)
                 .If($"color: { Color }", () => !string.IsNullOrEmpty(Color))
                 .AsString();

        #endregion

    }
}