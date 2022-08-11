using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class SimpleDialog : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public SimpleDialog()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public bool IsModal { get; set; }

        [Parameter]
        public string MaximumWidth { get; set; }

        [Parameter]
        public string MaximumHeight { get; set; }

        #endregion

        #region EVENTS

        protected Task OnClickDialogHandler(MouseEventArgs args)
        {
            if (IsModal)
                return Task.CompletedTask;

            IsOpen = false;
            return IsOpenChanged.InvokeAsync(IsOpen);
        }

        protected Task OnClickButtonCloseHandler(MouseEventArgs args)
        {
            IsOpen = false;
            return IsOpenChanged.InvokeAsync(IsOpen);
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-dialog")
                 .If("giz-dialog--open", () => IsOpen)
                 .AsString();
        protected string ContentStyleValue => new StyleMapper()
                 .If($"max-width: 100%;", () => string.IsNullOrEmpty(MaximumWidth))
                 .If($"max-width: {MaximumWidth};", () => !string.IsNullOrEmpty(MaximumWidth))
                 .If($"max-height: 100%;", () => string.IsNullOrEmpty(MaximumHeight))
                 .If($"max-height: {MaximumHeight};", () => !string.IsNullOrEmpty(MaximumHeight))
                 .AsString();

        #endregion

    }
}