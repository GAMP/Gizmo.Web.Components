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

        private bool _selected;

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [CascadingParameter]
        protected List Parent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public string Href { get; set; }

        [Parameter]
        public RenderFragment NestedList { get; set; }

        [Parameter]
        public bool Expanded { get; set; }

        protected void OnClickHandler(MouseEventArgs args)
        {
            if (Disabled)
                return;

            if (NestedList != null)
            {
                Expanded = !Expanded;
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
        }

        internal void SetSelected(bool selected)
        {
            if (Disabled)
                return;

            if (_selected == selected)
                return;

            _selected = selected;

            StateHasChanged();
        }

        protected string ClassName => new ClassMapper()
                 .Add("g-list-item")
                 .If("g-list-item-disabled", () => Disabled)
                 .If("g-list-item-selected", () => _selected).AsString();

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
                if (Parent == null)
                    return;

                Parent.Unregister(this);
            }
            catch (Exception) { }

            base.Dispose();
        }
    }
}