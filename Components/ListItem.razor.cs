﻿using Gizmo.Web.Components.Infrastructure;
using Microsoft.AspNetCore.Components;
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
        public RenderFragment NestedList { get; set; }

        [Parameter]
        public bool IsExpanded { get; set; }

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

        #endregion

        #region OVERRIDE

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

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-list-item")
                 .If("giz-list-item-disabled", () => IsDisabled)
                 .If("giz-list-item-selected", () => _isSelected).AsString();

        protected string StyleValue => new StyleMapper()
                 .If($"border: 1px solid {BorderColor}; border-radius: 0.4rem", () => HasBorder)
                 .AsString();

        #endregion

    }
}