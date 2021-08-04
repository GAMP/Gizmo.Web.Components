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

        private bool _selected;

        private TItemType _item;

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
        public bool IsOpen { get; set; }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
             .If("is-selected", () => _selected)
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

        protected async Task OnDataRowMouseEvent(MouseEventArgs args, TItemType item)
        {
            if (IsDropdown)
            {
                IsOpen = !IsOpen;
            }
            else
            {
                if (IsSelectable && Parent.SelectOnClick)
                {
                    await Parent?.SelectItem(this, !_selected);
                }
            }
        }

        protected async Task IsCheckedChangedHandler(bool value)
        {
            await Parent?.SelectItem(this, value);
        }

        protected async Task ContextMenuHandler(MouseEventArgs args)
        {
            if (IsSelectable && Parent.SelectOnClick)
            {
                await Parent?.SelectItem(this, !_selected);
            }

            if (args.Button == 2 && Parent.ContextMenu != null)
            {
                await Parent.OpenContextMenu(args.ClientX, args.ClientY);
            }
        }

        #endregion

        #region METHODS

        internal void SetSelected(bool selected)
        {
            if (!IsSelectable)
                return;

            if (_selected == selected)
                return;

            _selected = selected;

            StateHasChanged();
        }

        #endregion

    }
}