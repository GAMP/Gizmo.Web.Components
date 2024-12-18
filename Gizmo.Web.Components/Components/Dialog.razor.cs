using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Dialog : CustomDOMComponentBase
    {
        const int DEFAULT_DELAY = 300;

        #region CONSTRUCTOR
        public Dialog()
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
        public RenderFragment DialogHeader { get; set; }

        [Parameter]
        public RenderFragment DialogBody { get; set; }

        [Parameter]
        public RenderFragment DialogFooter { get; set; }

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
        public bool ShowCloseButton { get; set; }

        [Parameter]
        public bool Fade { get; set; } = true;

        #endregion

        #region EVENTS

        protected void OnClickDialogHandler(MouseEventArgs args)
        {
            if (IsModal)
                return;

            IsOpen = false;
        }

        protected void OnClickButtonCloseHandler(MouseEventArgs args)
        {
            IsOpen = false;
        }

        #endregion

        private Task FadeOut()
        {
            _isOpen = false;
            _isFadingOut = false;

            InvokeAsync(StateHasChanged);

            return IsOpenChanged.InvokeAsync(_isOpen);
        }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-dialog")
                 .If("giz-dialog--open", () => IsOpen)
                 .If("giz-dialog--fade", () => Fade)
                 .If("giz-dialog--fade-in", () => IsOpen && Fade && !_isFadingOut)
                 .If("giz-dialog--fade-out", () => IsOpen && Fade && _isFadingOut)
                 .AsString();

        protected string DialogStyleValue => new StyleMapper()
                 .If($"max-width: 100%;", () => string.IsNullOrEmpty(MaximumWidth))
                 .If($"max-width: {MaximumWidth};", () => !string.IsNullOrEmpty(MaximumWidth))
                 .If($"max-height: 100%;", () => string.IsNullOrEmpty(MaximumHeight))
                 .If($"max-height: {MaximumHeight};", () => !string.IsNullOrEmpty(MaximumHeight))
                 .AsString();

        #endregion

    }
}
