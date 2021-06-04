using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{

    public partial class GizInput : CustomDOMComponentBase
    {
        public enum InputSize
        {
            Normal = 0,
            Large = 1
        }

        #region CONSTRUCTOR
        public GizInput()
        {
        }
        #endregion

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
        public InputSize Size { get; set; } = InputSize.Normal;

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

        protected string ClassName => new ClassMapper()
                 .Add("giz-input-control")
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
                 .If("giz-input-root--large", () => Size == InputSize.Large)
                 .AsString();

        protected string ValidationClassName => new ClassMapper()
                 .If("giz-input-valid", () => IsValid)
                 .If("giz-input-invalid", () => !IsValid)
                 .AsString();
    }
}