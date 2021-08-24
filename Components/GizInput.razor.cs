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
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }

        [Parameter]
        public bool IsHidden { get; set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string LeftIcon { get; set; }

        [Parameter]
        public string RightIcon { get; set; }

        [Parameter]
        public Icons? LeftSVGIcon { get; set; }

        [Parameter]
        public Icons? RightSVGIcon { get; set; }

        [Parameter]
        public InputSizes Size { get; set; } = InputSizes.Normal;

        [Parameter]
        public bool HasOutline { get; set; } = true;

        [Parameter]
        public bool HasShadow { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

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
                 .If("giz-input-valid", () => IsValid)
                 .If("giz-input-invalid", () => !IsValid)
                 .AsString();

        protected string IconLeft => new ClassMapper()
                .Add("giz-input-icon-left")
                .AsString();

        protected string IconRight => new ClassMapper()
                .Add("giz-input-icon-right")
                .AsString();

        protected string FieldClassName => new ClassMapper()
                 .Add("giz-input-root")
                 .If("giz-input-root--outline", () => HasOutline)
                 .If("giz-input-root--shadow", () => HasShadow)
                 .If("giz-input-root--full-width", () => IsFullWidth)
                 .If("giz-input-root--small", () => Size == InputSizes.Small)
                 .If("giz-input-root--large", () => Size == InputSizes.Large)
                 .AsString();

        protected string ValidationClassName => new ClassMapper()
                 .Add("giz-input-validation")
                 .AsString();

        #endregion

    }
}