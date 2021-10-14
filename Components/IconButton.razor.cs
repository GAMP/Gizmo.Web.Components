using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class IconButton : ButtonBase
    {
        #region PROPERTIES

        [Parameter]
        public ButtonVariants Variant { get; set; } = ButtonVariants.Fill;

        /// <summary>
        /// Inline label of Button.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public Icons? SVGIcon { get; set; }

        [Parameter]
        public string IconColor { get; set; }

        [Parameter]
        public string IconBackgroundColor { get; set; }

        #endregion

        #region EVENTS

        protected Task OnClickButtonHandler(MouseEventArgs args)
        {
            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }

            return Task.CompletedTask;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-icon-button")
                 .Add($"giz-icon-button--{Size.ToDescriptionString()}")
                 .Add($"{Color.ToDescriptionString()}")
                 .If("giz-icon-button--fill", () => Variant == ButtonVariants.Fill)
                 .If("giz-icon-button--outline", () => Variant == ButtonVariants.Outline)
                 .If("giz-icon-button--text", () => Variant == ButtonVariants.Text)
                 .If("giz-icon-button-shadow", () => HasShadow)
                 .If("disabled", () => IsDisabled)
                 .AsString();

        protected string ButtonIcon => new ClassMapper()
                 .AsString();

        #endregion

    }
}