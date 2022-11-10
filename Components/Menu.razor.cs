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

        private List _popupContent;
        private bool _isOpen;

        private bool _shouldRender;

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
                _ = IsOpenChanged.InvokeAsync(_isOpen);

                _shouldRender = true;

                if (_isOpen)
                    _popupContent.Collapse();
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

        public bool ExpandBottomToTop { get; set; } = false;

        #endregion

        #region EVENTS

        protected async Task OnMouseDownHandler(MouseEventArgs args)
        {
            if (!IsContextMenu)
            {
                if (!IsDisabled)
                {
                    if ((ActivationEvent == MenuActivationEvents.LeftClick && args.Button == 0) ||
                        (ActivationEvent == MenuActivationEvents.RightClick && args.Button == 2))
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
        }

        public void OnMouseOverHandler(MouseEventArgs args)
        {
            if (ActivationEvent == MenuActivationEvents.MouseOver && !IsDisabled && !IsOpen)
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

        public void SetDirection(ListDirections direction)
        {
            Direction = direction;
        }

        public void Open()
        {
            IsOpen = true;

            StateHasChanged();
        }

        public void Open(double offsetX, double offsetY)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            IsOpen = true;

            StateHasChanged();
        }

        public void Close()
        {
            IsOpen = false;

            StateHasChanged();
        }

        public async Task<BoundingClientRect> GetListBoundingClientRect()
        {
            return await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", _popupContent.Ref);
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-menu")
                 .If("giz-menu--mouse", () => ActivationEvent == MenuActivationEvents.MouseOver)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-menu__dropdown")
                 .If("giz-menu__dropdown--cursor", () => IsContextMenu || OpenDirection == PopupOpenDirections.Cursor)
                 .AsString();

        protected string PopupStyleValue => new StyleMapper()
                 .If($"top: {OffsetY.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => IsContextMenu || OpenDirection == PopupOpenDirections.Cursor)
                 .If($"left: {OffsetX.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => IsContextMenu || OpenDirection == PopupOpenDirections.Cursor)
                 .AsString();

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                _shouldRender = false;
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }
    }
}