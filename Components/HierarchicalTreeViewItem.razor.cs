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

        private TItemType _item;
        private bool _isSelected = false;
        private bool _isExpanded = false;
        private bool _hasSubItems;
        private List<TItemType> _subItems;
        private HierarchicalTreeView<TItemType> _hierarchicalTreeView;

        #endregion

        #region PROPERTIES

        [CascadingParameter]
        protected HierarchicalTreeView<TItemType> Parent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public TItemType Item
        {
            get
            {
                return _item;
            }
            set
            {
                if (EqualityComparer<TItemType>.Default.Equals(_item, value))
                    return;

                _item = value;

                if (Parent != null)
                {
                    Parent.UpdateItem(this, _item);
                }
            }
        }

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
                _ = IsExpandedChanged.InvokeAsync(_isExpanded);
            }
        }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnDoubleClick { get; set; }

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

        internal bool IsChildSelected()
        {
            if (_isSelected)
                return true;

            if (_hasSubItems)
            {
                return _hierarchicalTreeView.IsChildSelected();
            }

            return false;
        }

        #endregion

        #region EVENTS

        protected async Task OnClickExpandButtonHandler(MouseEventArgs args)
        {
            //if (IsDisabled)
            //    return;

            IsExpanded = !IsExpanded;

            //If collapsed and the selected item is under this item then set this item as clicked item.
            if (!IsExpanded && IsChildSelected())
            {
                if (Parent != null)
                {
                    await Parent.SetClickedItem(this);
                }

                await OnClick.InvokeAsync(args);
            }
        }

        protected async Task OnClickHandler(MouseEventArgs args)
        {
            //if (IsDisabled)
            //    return;

            if (Parent != null)
            {
                await Parent.SetClickedItem(this);
            }

            await OnClick.InvokeAsync(args);
        }

        protected async Task OnDoubleClickEvent(MouseEventArgs args)
        {
            //if (IsDisabled)
            //    return;

            IsExpanded = !IsExpanded;

            if (Parent != null)
            {
                await Parent.SetDoubleClickedItem(this);
            }

            await OnDoubleClick.InvokeAsync(args);
        }

        protected async Task ContextMenuHandler(MouseEventArgs args)
        {
            if (Parent != null)
            {
                if (args.Button == 2)
                {
                    await Parent.SetClickedItem(this);
                    await Parent.OpenContextMenu(args.ClientX, args.ClientY);
                }
            }
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-hierarchical-tree-view-item")
                 .If("selected", () => _isSelected)
                 .If("giz-hierarchical-tree-view-item--expanded", () => IsExpanded)
                 .AsString();

        #endregion

        #region OVERRIDES

        protected override Task OnInitializedAsync()
        {
            if (Parent != null)
            {
                Parent.Register(this, Item);
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
                    Parent.Unregister(this, Item);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }

        #endregion

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (!firstRender)
        //    {
        //        await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
        //    }

        //    await base.OnAfterRenderAsync(firstRender);
        //}
    }
}
