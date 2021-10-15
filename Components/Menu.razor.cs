using Gizmo.Web.Components.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Menu : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Menu()
        {
        }
        #endregion

        #region FIELDS

        private List _itemsList;
        private bool _isOpen;

        #endregion

        #region PROPERTIES

        [Parameter]
        public bool CloseOnItemClick { get; set; } = true;

        [Parameter]
        public double OffsetX { get; set; }

        [Parameter]
        public double OffsetY { get; set; }

        [Parameter]
        public bool IsContextMenu { get; set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public ButtonVariants Variant { get; set; } = ButtonVariants.Outline;

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public Icons? SVGIcon { get; set; }

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
                    _itemsList.Collapse();
            }
        }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public RenderFragment Activator { get; set; }

        [Parameter]
        public MenuActivationEvents ActivationEvent { get; set; } = MenuActivationEvents.LeftClick;

        [Parameter]
        public ListDirections Direction { get; set; } = ListDirections.Right;

        [Parameter]
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        [Parameter]
        public bool PreserveIconSpace { get; set; }

        [Parameter]
        public PopupOpenDirections OpenDirection { get; set; } = PopupOpenDirections.Bottom;

        internal bool ExpandBottomToTop { get; set; } = false;

        #endregion

        #region EVENTS

        protected async Task OnMouseDownHandler(MouseEventArgs args)
        {
            if ((ActivationEvent == MenuActivationEvents.LeftClick && args.Button == 0) ||
               (ActivationEvent == MenuActivationEvents.RightClick && args.Button == 2))
            {
                if (!IsDisabled)
                {
                    if (OpenDirection == PopupOpenDirections.Cursor)
                    {
                        var windowSize = await JsInvokeAsync<WindowSize>("getWindowSize");
                        var mainMenuSize = await this.GetListBoundingClientRect();

                        if (args.ClientX > windowSize.Width / 2)
                        {
                            //Open direction right to left.
                            OffsetX = args.ClientX - mainMenuSize.Width;
                            Direction = ListDirections.Left;
                        }
                        else
                        {
                            OffsetX = args.ClientX;
                            Direction = ListDirections.Right;
                        }

                        if (args.ClientY > windowSize.Height / 2)
                        {
                            //Open direction bottom to top.
                            OffsetY = args.ClientY - mainMenuSize.Height;
                            ExpandBottomToTop = true;
                        }
                        else
                        {
                            OffsetY = args.ClientY;
                            ExpandBottomToTop = false;
                        }
                    }

                    IsOpen = !IsOpen;
                }
            }
        }

        public void OnMouseOverHandler(MouseEventArgs args)
        {
            if (ActivationEvent == MenuActivationEvents.MouseOver && !IsDisabled)
            {
                if (OpenDirection == PopupOpenDirections.Cursor)
                {
                    OffsetX = args.ClientX;
                    OffsetY = args.ClientY;
                }

                IsOpen = true;
            }
        }

        public void OnMouseLeaveHandler(MouseEventArgs args)
        {
            if (ActivationEvent == MenuActivationEvents.MouseOver && !IsDisabled)
                IsOpen = false;
        }

        protected Task OnClickMenuItemHandler()
        {
            if (!IsDisabled && CloseOnItemClick)
                IsOpen = false;

            return Task.CompletedTask;
        }

        protected Task OnClickOverlayHandler(MouseEventArgs args)
        {
            IsOpen = false;

            return Task.CompletedTask;
        }

        protected Task OnContextMenuHandler(MouseEventArgs args)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region METHODS

        internal void Close()
        {
            IsOpen = false;
        }

        internal async Task<BoundingClientRect> GetListBoundingClientRect()
        {
            return await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", _itemsList.Ref);
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-menu")
                 .AsString();

        protected string PopupWrapperClassName => new ClassMapper()
                 .If("giz-popup-wrapper", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If("giz-popup-wrapper--visible", () => OpenDirection == PopupOpenDirections.Cursor && IsOpen)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-menu__dropdown")
                 .If("giz-menu__dropdown--cursor", () => IsContextMenu || OpenDirection == PopupOpenDirections.Cursor)
                 .AsString();

        protected string PopupStyleValue => new StyleMapper()
                 .If($"top: {OffsetY}px", () => IsContextMenu || OpenDirection == PopupOpenDirections.Cursor)
                 .If($"left: {OffsetX}px", () => IsContextMenu || OpenDirection == PopupOpenDirections.Cursor)
                 .AsString();

        #endregion

    }
}