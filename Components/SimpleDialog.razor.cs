using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class SimpleDialog : CustomDOMComponentBase
    {
        const int DEFAULT_DELAY = 300;

        #region CONSTRUCTOR
        public SimpleDialog()
        {
            _deferredAction = new DeferredAction(FadeOut);
            _delayTimeSpan = new TimeSpan(0, 0, 0, 0, _delay);
        }
        #endregion

        private bool _isOpen;
        private bool _isFadingOut;

        private DeferredAction _deferredAction;
        private int _delay = DEFAULT_DELAY;
        private TimeSpan _delayTimeSpan;

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

                if (value)
                {
                    if (_isFadingOut)
                    {
                        _isFadingOut = false;
                        _deferredAction.Cancel();
                    }

                    _isOpen = value;
                    _ = IsOpenChanged.InvokeAsync(_isOpen);
                }
                else
                {
                    if (!_isFadingOut)
                    {
                        _isFadingOut = true;
                        _deferredAction.Defer(_delayTimeSpan);
                    }
                }
            }
        }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public bool IsModal { get; set; }

        [Parameter]
        public string MaximumWidth { get; set; }

        [Parameter]
        public string MaximumHeight { get; set; }

        [Parameter]
        public bool Fade { get; set; } = true;

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

        private Task FadeOut()
        {
            InvokeAsync(Close);

            return Task.CompletedTask;
        }

        private Task Close()
        {
            _isOpen = false;
            _isFadingOut = false;

            StateHasChanged();

            return IsOpenChanged.InvokeAsync(_isOpen);
        }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-dialog")
                 .If("giz-dialog--open", () => IsOpen)
                 .If("fade", () => Fade)
                 .If("fade-in", () => IsOpen && Fade && !_isFadingOut)
                 .If("fade-out", () => IsOpen && Fade && _isFadingOut)
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