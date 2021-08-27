using Gizmo.Web.Components.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Popup : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Popup()
        {
        }
        #endregion

        private bool _isOpen;

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;
                IsOpenChanged.InvokeAsync(_isOpen);

                if (_isOpen)
                {
                    Task.Run(async () =>
                    {
                        await Focus();
                    });
                }
            }
        }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public string MaximumHeight { get; set; }

        [Parameter]
        public bool IsModal { get; set; }

        #endregion

        #region METHODS

        private async Task Focus()
        {
            await InvokeVoidAsync("focusElement", Ref);
        }

        #endregion

        #region EVENTS

        protected Task OnClickPopupHandler(MouseEventArgs args)
        {
            if (!IsModal)
                IsOpen = false;

            return Task.CompletedTask;
        }

        protected Task OnFocusOutHandler()
        {
            if (!IsModal)
                IsOpen = false;

            return Task.CompletedTask;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-popup")
                 .If("giz-popup-open", () => IsOpen)
                 .AsString();

        protected string StyleValue => new StyleMapper()
                 .If($"max-height: {MaximumHeight}", () => !string.IsNullOrEmpty(MaximumHeight))
                 .AsString();

        #endregion

    }
}