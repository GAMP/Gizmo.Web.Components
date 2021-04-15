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
        public bool IsDropdown { get; set; }

        [Parameter]
        public bool Open { get; set; }

        internal async ValueTask OnDataRowMouseEvent(MouseEventArgs args, TItemType item)
        {
            if (IsDropdown)
            {
                Open = !Open;
            }
            else
            {
                Parent?.SetSelectedItem(this);
            }
        }

        internal void SetSelected(bool selected)
        {
            if (_selected == selected)
                return;

            _selected = selected;

            StateHasChanged();
        }

        protected string ClassName => new ClassMapper()
             .If("is-selected", () => _selected)
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