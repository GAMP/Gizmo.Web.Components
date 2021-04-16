using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class DataGridRow<TItemType> : CustomDOMComponentBase
    {
        private bool _selected;

        [CascadingParameter(Name = "Parent")]
        protected DataGrid<TItemType> Parent { get; set; }

        [Parameter]
        public IEnumerable<DataGridColumn<TItemType>> Columns { get; set; }

        [Parameter]
        public TItemType Item { get; set; }

        [Parameter]
        public bool IsSelectable { get; set; }

        [Parameter]
        public bool IsDropdown { get; set; }

        [Parameter]
        public bool Open { get; set; }

        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
            }
        }

        internal async Task IsCheckedChangedHandler(bool value)
        {
            await Parent?.SetSelectedItem(this);
        }

        internal async Task OnDataRowMouseEvent(MouseEventArgs args, TItemType item)
        {
            if (IsDropdown)
            {
                Open = !Open;
            }
            else
            {
                if (IsSelectable)
                {
                    //await Parent?.SetSelectedItem(this);
                }
            }
        }

        internal void SetSelected(bool selected)
        {
            if (!IsSelectable)
                return;

            if (Selected == selected)
                return;

            Selected = selected;

            StateHasChanged();
        }

        protected string ClassName => new ClassMapper()
             .If("is-selected", () => Selected)
             .If("table__row-dropdown", () => IsDropdown)
             .If("is-opened", () => Open).AsString();

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

    }
}