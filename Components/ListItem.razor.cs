using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;

namespace Gizmo.Web.Components
{
    public partial class ListItem : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public ListItem()
        {
        }
        #endregion

        #region MEMBERS

        private bool _isSelected;

        #endregion

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [CascadingParameter]
        protected List Parent { get; set; }

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public string Href { get; set; }

        [Parameter]
        public RenderFragment NestedList { get; set; }

        [Parameter]
        public bool IsExpanded { get; set; }
        
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }
               
        #endregion

        #region EVENTS

        protected void OnClickHandler(MouseEventArgs args)
        {
            if (IsDisabled)
                return;

            if (NestedList != null)
            {
                IsExpanded = !IsExpanded;
            }
            else if (Href != null)
            {
                Parent?.SetSelectedItem(this);
                NavigationManager.NavigateTo(Href);
            }
            else
            {
                Parent?.SetSelectedItem(this);
            }

            OnClick.InvokeAsync(args);
        }

        #endregion

        internal void SetSelected(bool selected)
        {
            if (IsDisabled)
                return;

            if (_isSelected == selected)
                return;

            _isSelected = selected;

            StateHasChanged();
        }

        protected string ClassName => new ClassMapper()
                 .Add("giz-list-item")                
                 .If("giz-list-item-disabled", () => IsDisabled)
                 .If("giz-list-item-selected", () => _isSelected).AsString();

        protected override void OnInitialized()
        {
            if (Parent != null)
            {
                Parent.Register(this);
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
    }
}