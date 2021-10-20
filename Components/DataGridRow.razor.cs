using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class DataGridRow<TItemType> : CustomDOMComponentBase
    {
        #region FIELDS

        private bool _isSelected;
        private TItemType _item;
        private bool _isOpen;
        private bool _shouldRender;

        #endregion

        #region PROPERTIES

        [CascadingParameter(Name = "Parent")]
        protected DataGrid<TItemType> Parent { get; set; }

        [Parameter]
        public IEnumerable<DataGridColumn<TItemType>> Columns { get; set; }

        [Parameter]
        public TItemType Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;

                if (Parent != null)
                {
                    Parent.UpdateRow(this, _item);
                }
            }
        }

        [Parameter]
        public bool IsSelectable { get; set; }

        [Parameter]
        public bool IsDropdown { get; set; }

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

                _shouldRender = true;
            }
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
             .If("is-selected", () => _isSelected)
             .If("table__row-dropdown", () => IsDropdown)
             .If("is-opened", () => IsOpen).AsString();

        #endregion

        #region OVERRIDES

        protected override Task OnInitializedAsync()
        {
            if (Parent != null)
            {
                Parent.AddRow(this, Item);
            }

            return base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            try
            {
                if (Parent != null)
                {
                    Parent.RemoveRow(this, Item);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }

        #endregion

        #region EVENTS

        protected async Task OnClickEvent(MouseEventArgs args, TItemType item)
        {
            //TODO: A ADD DELAY FOR DOUBLE CLICK?
            if (IsDropdown)
            {
                IsOpen = !IsOpen;
            }
            else
            {
                if (IsSelectable && Parent.SelectOnClick)
                {
                    await Parent?.SelectRow(this, !_isSelected);
                }
            }
        }

        protected async Task OnDoubleClickEvent(MouseEventArgs args, TItemType item)
        {
            await Parent?.DoubleClickRow(this);
        }

        protected async Task IsCheckedChangedHandler(bool value)
        {
            await Parent?.SelectRow(this, value);
        }

        protected async Task ContextMenuHandler(MouseEventArgs args)
        {
            if (Parent != null)
            {
                if (IsSelectable && Parent.SelectOnClick)
                {
                    await Parent?.SelectRow(this, !_isSelected);
                }

                if (args.Button == 2 && Parent.ContextMenu != null)
                {
                    await Parent.SetActiveItem(Item);
                    await Parent.OpenContextMenu(args.ClientX, args.ClientY);
                }
            }
        }

        #endregion

        #region METHODS

        public void Refresh()
        {
            _shouldRender = true;

            StateHasChanged();
        }

        internal void SetSelected(bool selected)
        {
            if (!IsSelectable)
                return;

            if (_isSelected == selected)
                return;

            _isSelected = selected;
            _shouldRender = true;

            StateHasChanged();
        }

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