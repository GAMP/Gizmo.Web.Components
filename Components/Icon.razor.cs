using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

        #region CONSTRUCTOR
        public Icon()
        {
            ClassMapper
                .If("fa-stack", () => BackgroundStyle != IconBackgroundStyles.None);

            IconClassMapper
                .If("fa-stack-1x", () => BackgroundStyle != IconBackgroundStyles.None);
        }
        #endregion

        private ClassMapper _iconClassMapper;

        protected ClassMapper IconClassMapper
        {
            get
            {
                if (_iconClassMapper == null)
                    _iconClassMapper = new ClassMapper();

                return _iconClassMapper;
            }
        }

        [Parameter]
        public string Source { get; set; }

        [Parameter]
        public string Size { get; set; }

        [Parameter]
        public string Color { get; set; }

        [Parameter]
        public IconBackgroundStyles BackgroundStyle { get; set; } = IconBackgroundStyles.None;

        [Parameter]
        public string BackgroundColor { get; set; }
    }
}