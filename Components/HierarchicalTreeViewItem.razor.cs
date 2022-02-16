using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class HierarchicalTreeViewItem<TItemType> : CustomDOMComponentBase
    {
        #region FIELDS

        private bool _isSelected = false;
        private bool _isExpanded = false;
        private bool _hasSubItems;
        private List<TItemType> _subItems;

        #endregion

        #region PROPERTIES

        [CascadingParameter]
        protected HierarchicalTreeView<TItemType> Parent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public TItemType Item { get; set; }

        [Parameter]
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (_isExpanded == value)
                    return;

                _isExpanded = value;

                IsExpandedChanged.InvokeAsync(_isExpanded);
            }
        }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public EventCallback<bool> IsExpandedChanged { get; set; }

        #endregion

        #region METHODS

        internal void SetSelected(bool value)
        {
            //if (IsDisabled)
            //    return;

            if (_isSelected == value)
                return;

            _isSelected = value;

            StateHasChanged();
        }

        internal void SetExpanded(bool expanded)
        {
            //if (IsDisabled)
            //return;

            IsExpanded = expanded;

            //StateHasChanged();
        }

        #endregion

        #region EVENTS

        protected async Task OnClickHandler(MouseEventArgs args)
        {
            //if (IsDisabled)
            //    return;

            IsExpanded = !IsExpanded;

            if (Parent != null)
            {
                await Parent.SetClickedItem(this, IsExpanded);
            }

            await OnClick.InvokeAsync(args);
        }

        protected async Task ContextMenuHandler(MouseEventArgs args)
        {
            if (Parent != null)
            {
                if (args.Button == 2)
                {
                    await Parent.SetClickedItem(this, IsExpanded);
                    await Parent.OpenContextMenu(args.ClientX, args.ClientY);
                }
            }
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-tree-view-item")
                 .If("giz-tree-view-item--selected", () => _isSelected)
                 .If("giz-tree-view-item--expanded", () => IsExpanded)
                 .AsString();

        #endregion

        #region OVERRIDES

        protected override Task OnInitializedAsync()
        {
            if (Parent != null)
            {
                Parent.Register(this);
                //IsDisabled = Parent.IsDisabled;

                if (Item != null && !string.IsNullOrEmpty(Parent.HierarchicalItemSource))
                {
                    try
                    {
                        var property = typeof(TItemType).GetProperty(Parent.HierarchicalItemSource);
                        _subItems = (List<TItemType>)property.GetValue(Item);
                        if (_subItems != null && _subItems.Count > 0)
                        {
                            _hasSubItems = true;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return base.OnInitializedAsync();
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

    }
}