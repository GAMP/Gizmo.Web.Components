using Gizmo.Web.Components.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gizmo.Web.Components
{
    public partial class ListItem : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public ListItem()
        {
        }
        #endregion

        #region FIELDS

        private bool _isSelected;

        #endregion

        #region PROPERTIES

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [CascadingParameter]
        protected List Parent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public Icons? SVGIcon { get; set; }

        [Parameter]
        public string Href { get; set; }

        [Parameter]
        public NavLinkMatch Match { get; set; } = NavLinkMatch.All;

        [Parameter]
        public RenderFragment NestedList { get; set; }

        [Parameter]
        public bool HasBorder { get; set; }

        [Parameter]
        public string BorderColor { get; set; } = "#edeef2";

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public ICommand Command { get; set; }

        [Parameter]
        public object CommandParameter { get; set; }

        [Parameter]
        public string BackgroundImage { get; set; }

        [Parameter]
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected == value)
                    return;

                _isSelected = value;

                if (Parent != null)
                    _ = Parent.SetSelectedItem(this);
            }
        }

        public bool IsExpanded { get; set; }

        #endregion

        #region EVENTS

        protected async Task OnClickHandler(MouseEventArgs args)
        {
            if (IsDisabled)
                return;

            if (NestedList != null)
            {
                IsExpanded = !IsExpanded;
            }
            else if (Href != null)
            {
                await Parent?.SetClickedItem(this);
                NavigationManager.NavigateTo(Href);
            }
            else
            {
                if (Parent != null)
                    await Parent.SetClickedItem(this);
            }

            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }

            await OnClick.InvokeAsync(args);
        }

        private async void NavigationManager_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            if (Href != null)
            {
                if (IsActiveLink())
                    await Parent.SetSelectedItem(this);
            }
        }

        #endregion

        #region METHODS

        internal void SetSelected(bool selected)
        {
            if (IsDisabled)
                return;

            if (_isSelected == selected)
                return;

            _isSelected = selected;

            StateHasChanged();
        }

        private bool IsActiveLink()
        {
            var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri).ToLower();

            if (Match == NavLinkMatch.All)
                return relativePath == Href.ToLower();
            else
                return relativePath.StartsWith(Href.ToLower());
        }

        #endregion

        #region OVERRIDE

        protected override async Task OnInitializedAsync()
        {
            NavigationManager.LocationChanged += NavigationManager_LocationChanged;

            if (Parent != null)
            {
                Parent.Register(this);

                if (Href != null)
                {
                    if (IsActiveLink())
                        await Parent.SetSelectedItem(this);
                }
                else
                {
                    if (_isSelected)
                    {
                        await Parent.SetSelectedItem(this);
                    }
                }
            }
        }

        public override void Dispose()
        {
            try
            {
                if (Parent != null)
                {
                    Parent.Unregister(this);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-list-item")
                 .If("giz-list-item-disabled", () => IsDisabled)
                 .If("giz-list-item-selected", () => _isSelected).AsString();

        protected string StyleValue => new StyleMapper()
                 .If($"border: 1px solid {BorderColor}; border-radius: 0.4rem", () => HasBorder)
                 .If($"background: url('{BackgroundImage}') no-repeat; background-size: cover;", () => !string.IsNullOrEmpty(BackgroundImage))
                 .AsString();

        #endregion

    }
}