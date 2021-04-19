using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    /// <summary>
    /// Data grid component.
    /// </summary>
    public partial class DataGrid<TItemType> : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public DataGrid()
        {
            ClassMapper.Add("table");
        }
        #endregion

        #region FIELDS

        private readonly HashSet<DataGridColumn<TItemType>> _columns = new(Enumerable.Empty<DataGridColumn<TItemType>>());
        private ICollection<TItemType> _selectedItems = new HashSet<TItemType>();
        internal Dictionary<TItemType, DataGridRow<TItemType>> _rows = new Dictionary<TItemType, DataGridRow<TItemType>>();

        private bool _hasSelectedItems;
        private bool _hasSelectedAllItems;
        private int _providerTotalItems = 0;

        #endregion

        #region PROPERTIES

        #region PUBLIC

        [Parameter]
        public bool IsSelectable { get; set; }

        /// <summary>
        /// Gets or sets if virtualization enabled.
        /// </summary>
        [Parameter()]
        public bool IsVirtualized
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets item source.
        /// </summary>
        [Parameter()]
        public ICollection<TItemType> ItemSource
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets item size.
        /// </summary>
        /// <remarks>
        /// Only applicable if virtualization is enabled.
        /// </remarks>
        [Parameter()]
        public int ItemSize
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets overscan count.
        /// </summary>
        /// <remarks>
        /// Only applicable if virtualization is enabled.
        /// </remarks>
        [Parameter()]
        public int OverscanCount { get; set; } = 3;

        /// <summary>
        /// Gets or sets items provider delegate.
        /// </summary>
        /// <remarks>
        /// Only applicable if virtualization is enabled.
        /// </remarks>
        [Parameter()]
        public ItemsProviderDelegate<TItemType> ItemsProvider
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets placeholder template.
        /// </summary>
        /// <remarks>
        /// Only applicable if virtualization is enabled.
        /// </remarks>
        [Parameter()]
        public RenderFragment PlaceHolderTemplate
        {
            get; set;
        }

        /// <summary>
        /// Gets columns collection.
        /// </summary>
        public IEnumerable<DataGridColumn<TItemType>> Columns
        {
            get { return _columns; }
        }

        /// <summary>
        /// Gets or sets selected item.
        /// </summary>
        [Parameter()]
        public TItemType SelectedItem
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets selected items.
        /// </summary>
        [Parameter()]
        public ICollection<TItemType> SelectedItems
        {
            get { return _selectedItems; }
            set { _selectedItems = value; }
        }

        /// <summary>
        /// Gets or sets selection mode.
        /// </summary>
        [Parameter()]
        public SelectionMode SelectionMode
        {
            get;
            set;
        }

        [Parameter()]
        public RenderFragment DetailTemplate
        {
            get; set;
        }

        [Parameter()]
        public RenderFragment ChildContent
        {
            get; set;
        }

        [Parameter()]
        public EventCallback<TItemType> SelectedItemChanged
        {
            get; set;
        }

        [Parameter()]
        public EventCallback<ICollection<TItemType>> SelectedItemsChanged
        {
            get; set;
        }

        #endregion

        #endregion

        protected override async Task OnFirstAfterRenderAsync()
        {
            if (IsVirtualized && ItemsProvider != null)
            {
                var itemsProviderResult = await ItemsProvider.Invoke(new ItemsProviderRequest());
                _providerTotalItems = itemsProviderResult.TotalItemCount;
            }
        }

        protected void IsCheckedChangedHandler(bool value)
        {
            if (_hasSelectedAllItems)
            {
                SelectedItems?.Clear();

                //Set all items selected property to false.
                foreach (var row in _rows.ToArray())
                {
                    row.Value.SetSelected(false);
                }

                _hasSelectedItems = false;
                _hasSelectedAllItems = false;
            }
            else
            {
                //Set all items selected property to true.
                foreach (var row in _rows.ToArray())
                {
                    SelectedItems.Add(row.Key);
                    row.Value.SetSelected(true);
                }

                _hasSelectedItems = true;
                _hasSelectedAllItems = true;
            }
        }

        #region INTERNAL

        internal void AddColumn(DataGridColumn<TItemType> column)
        {
            _columns.Add(column);
        }

        internal void RemoveColumn(DataGridColumn<TItemType> column)
        {
            _columns.Remove(column);
        }

        internal void AddRow(DataGridRow<TItemType> row, TItemType item)
        {
            if (item == null)
                return;

            _rows[item] = row;

            if (SelectedItems.Contains(item))
                _rows[item].SetSelected(true);
        }

        internal void RemoveRow(DataGridRow<TItemType> row, TItemType item)
        {
            if (item == null)
                return;

            _rows.Remove(item);
        }

        internal async Task SelectItem(DataGridRow<TItemType> item, bool selected)
        {
            //called once data row item is clicked

            TItemType dataItem = item.Item;

            //no matter of selection the clicked item is always the selected one
            SelectedItem = dataItem;

            bool wasSelected = SelectedItems?.Contains(dataItem) == true;

            if (wasSelected == selected)
                return;

            if (SelectionMode == SelectionMode.Single)
            {
                //In single selection mode.

                //If current row is already selected.
                if (wasSelected)
                {
                    //Clear selected items list and set selected property to false.
                    SelectedItems?.Clear();
                    item.SetSelected(false);
                }
                else
                {
                    //Clear selected items list, add only this item in the list and set selected property to true.
                    SelectedItems?.Clear();
                    SelectedItems?.Add(dataItem);
                    item.SetSelected(true);

                    //Set all other items selected property to false.
                    foreach (var row in _rows.Where(a => a.Value != item).ToArray())
                    {
                        row.Value.SetSelected(false);
                    }
                }
            }
            else if (SelectionMode == SelectionMode.Extended)
            {
                //In extended selection mode.

                //If current row is already selected.
                if (wasSelected)
                {
                    //Remove current row from selected items list and set selected property to false.
                    SelectedItems.Remove(dataItem);
                    item.SetSelected(false);
                }
                else
                {
                    //Add current row in selected items list and set selected property to true.
                    SelectedItems?.Add(dataItem);
                    item.SetSelected(true);
                }
            }

            if (SelectedItems?.Count == 0)
            {
                _hasSelectedItems = false;
                SelectedItem = default;
            }
            else
            {
                _hasSelectedItems = true;
                if (ItemSource != null)
                {
                    if (SelectedItems?.Count == ItemSource.Count)
                    {
                        _hasSelectedAllItems = true;
                    }
                    else
                    {
                        _hasSelectedAllItems = false;
                    }
                }
                else
                {
                    if (SelectedItems?.Count == _providerTotalItems)
                    {
                        _hasSelectedAllItems = true;
                    }
                    else
                    {
                        _hasSelectedAllItems = false;
                    }
                }
            }

            StateHasChanged();

            await SelectedItemChanged.InvokeAsync(dataItem);
            await SelectedItemsChanged.InvokeAsync();
        }

        internal ValueTask OnHeaderRowMouseEvent(MouseEventArgs args, DataGridColumn<TItemType> column)
        {
            return ValueTask.CompletedTask;
        }

        #endregion
    }
}