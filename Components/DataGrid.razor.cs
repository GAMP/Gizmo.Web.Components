using Gizmo.Web.Components.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        }
        #endregion

        #region FIELDS

        private readonly HashSet<DataGridColumn<TItemType>> _columns = new(Enumerable.Empty<DataGridColumn<TItemType>>());
        private ICollection<TItemType> _selectedItems = new HashSet<TItemType>();
        internal Dictionary<TItemType, DataGridRow<TItemType>> _rows = new Dictionary<TItemType, DataGridRow<TItemType>>();

        private bool _hasSelectedItems;
        private bool _hasSelectedAllItems;
        private int _providerTotalItems = 0;
        //private bool _isContextMenuOpen;
        private double _clientX;
        private double _clientY;
        private Menu _contextMenu;
        private DataGridColumn<TItemType> _sortColumn;
        private SortDirections _sortDirection;
        private TItemType _activeItem;
        private ICollection<TItemType> _itemSource;

        #endregion

        #region PROPERTIES

      [Parameter]
        public DataGridVariants Variant { get; set; } = DataGridVariants.Default;

        [Parameter]
        public bool HasStickyHeader { get; set; }

        [Parameter]
        public bool IsSelectable { get; set; }

        [Parameter]
        public bool ShowCheckBoxes { get; set; }

        [Parameter]
        public bool SelectOnClick { get; set; }

        /// <summary>
        /// Gets or sets if virtualization enabled.
        /// </summary>
        [Parameter]
        public bool IsVirtualized { get; set; }

        /// <summary>
        /// Gets or sets item source.
        /// </summary>
        [Parameter]
        public ICollection<TItemType> ItemSource
        {
            get
            {
                return _itemSource;
            }
            set
            {
                if (_itemSource == value)
                    return;

                _itemSource = value;

                _shouldRender = true;
            }
        }

        /// <summary>
        /// Gets or sets item size.
        /// </summary>
        /// <remarks>
        /// Only applicable if virtualization is enabled.
        /// </remarks>
        [Parameter]
        public int ItemSize { get; set; }

        /// <summary>
        /// Gets or sets overscan count.
        /// </summary>
        /// <remarks>
        /// Only applicable if virtualization is enabled.
        /// </remarks>
        [Parameter]
        public int OverscanCount { get; set; } = 3;

        /// <summary>
        /// Gets or sets items provider delegate.
        /// </summary>
        /// <remarks>
        /// Only applicable if virtualization is enabled.
        /// </remarks>
        [Parameter]
        public ItemsProviderDelegate<TItemType> ItemsProvider { get; set; }

        /// <summary>
        /// Gets or sets placeholder template.
        /// </summary>
        /// <remarks>
        /// Only applicable if virtualization is enabled.
        /// </remarks>
        [Parameter]
        public RenderFragment PlaceHolderTemplate { get; set; }

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
        [Parameter]
        public TItemType SelectedItem { get; set; }

        /// <summary>
        /// The item under mouse on right click.
        /// </summary>
        [Parameter]
        public TItemType ActiveItem
        {
            get
            {
                return _activeItem;
            }
            set
            {
                if (EqualityComparer<TItemType>.Default.Equals(_activeItem, value))
                    return;

                _activeItem = value;

                _ = ActiveItemChanged.InvokeAsync(_activeItem);
            }
        }

        [Parameter]
        public string RowClass { get; set; }

        /// <summary>
        /// Gets or sets selected items.
        /// </summary>
        [Parameter]
        public ICollection<TItemType> SelectedItems
        {
            get { return _selectedItems; }
            set { _selectedItems = value; }
        }

        /// <summary>
        /// Gets or sets selection mode.
        /// </summary>
        [Parameter]
        public SelectionMode SelectionMode { get; set; }

        [Parameter]
        public RenderFragment<TItemType> DetailTemplate { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback<TItemType> SelectedItemChanged { get; set; }

        [Parameter]
        public EventCallback<ICollection<TItemType>> SelectedItemsChanged { get; set; }

        [Parameter]
        public EventCallback<TItemType> ActiveItemChanged { get; set; }

        [Parameter]
        public EventCallback<TItemType> OnDoubleClickItem { get; set; }

        [Parameter]
        public RenderFragment ContextMenu { get; set; }

        [Parameter]
        public bool ShowColumnSelector { get; set; }

        [Parameter]
        public string TableClass { get; set; }

        #endregion

        #region OVERRIDE

        protected override async Task OnFirstAfterRenderAsync()
        {
            if (IsVirtualized && ItemsProvider != null)
            {
                var itemsProviderResult = await ItemsProvider.Invoke(new ItemsProviderRequest());
                _providerTotalItems = itemsProviderResult.TotalItemCount;
            }
        }

        #endregion

        #region EVENTS

        protected async Task IsCheckedChangedHandler(bool value)
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

            await SelectedItemChanged.InvokeAsync(SelectedItems.FirstOrDefault());
            await SelectedItemsChanged.InvokeAsync();
        }

        internal ValueTask OnHeaderRowMouseEvent(MouseEventArgs args, DataGridColumn<TItemType> column)
        {
            if (column.CanSort)
            {
                foreach (var item in Columns)
                {
                    if (item != column)
                    {
                        item.IsSorted = false;
                    }
                }

                if (_sortColumn != column)
                {
                    _sortColumn = column;
                    _sortDirection = SortDirections.Ascending;
                    column.SortDirection = _sortDirection;

                    column.IsSorted = true;
                }
                else
                {
                    if (_sortDirection == SortDirections.Ascending)
                    {
                        _sortDirection = SortDirections.Descending;
                    }
                    else
                    {
                        _sortDirection = SortDirections.Ascending;
                    }
                    column.SortDirection = _sortDirection;

                    column.IsSorted = true;
                }

                StateHasChanged();
            }

            return ValueTask.CompletedTask;
        }

        #endregion

        #region METHODS

        internal Task SetActiveItem(TItemType item)
        {
            ActiveItem = item;

            return Task.CompletedTask;
        }

        internal async Task OpenContextMenu(double clientX, double clientY)
        {
            var gridPosition = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", Ref);

            var windowSize = await JsInvokeAsync<WindowSize>("getWindowSize");
            var contextMenuSize = await _contextMenu.GetListBoundingClientRect();

            _clientX = clientX - gridPosition.Left;
            if (gridPosition.Left + _clientX + contextMenuSize.Width > windowSize.Width)
            {
                _clientX = windowSize.Width - gridPosition.Left - contextMenuSize.Width - 40;
            }

            _clientY = clientY - gridPosition.Top;

            _contextMenu.Open(_clientX, _clientY);

            //_isContextMenuOpen = true;

            //StateHasChanged();
        }

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

        internal void UpdateRow(DataGridRow<TItemType> row, TItemType item)
        {
            if (item == null)
                return;

            _rows[item] = row;

            if (SelectedItems.Contains(item))
                _rows[item].SetSelected(true);
            else
                _rows[item].SetSelected(false);
        }

        internal void RemoveRow(DataGridRow<TItemType> row, TItemType item)
        {
            if (item == null)
                return;

            _rows.Remove(item);
        }

        internal async Task DoubleClickRow(DataGridRow<TItemType> item)
        {
            TItemType dataItem = item.Item;

            await OnDoubleClickItem.InvokeAsync(dataItem);
        }

        internal async Task SelectRow(DataGridRow<TItemType> item, bool selected)
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

            //StateHasChanged();

            await SelectedItemChanged.InvokeAsync(dataItem);
            await SelectedItemsChanged.InvokeAsync();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .If("giz-data-grid-wrapper", () => Variant == DataGridVariants.Default)
                 .If("giz-data-grid-sticky-header", () => HasStickyHeader)
                 .AsString();

        protected string TableClassName => new ClassMapper()
                 .If("giz-data-grid", () => Variant == DataGridVariants.Default)
                 .AsString();

        #endregion

        private IEnumerable<TItemType> GetSortedData()
        {
            if (_sortColumn == null)
                return ItemSource;

            if (_sortColumn.SortFunction != null)
            {
                return _sortColumn.SortFunction.Invoke(_sortDirection);
            }
            else
            {
                Comparer comparer = new Comparer(_sortColumn.Field, _sortDirection);
                List<TItemType> tmp = ItemSource.ToList();
                tmp.Sort(comparer);
                return tmp;
            }
        }

        public class Comparer : IComparer<TItemType>
        {
            private string _sortColumn;
            private SortDirections _sortDirection;

            public Comparer(string sortColumn, SortDirections sortDirections)
            {
                _sortColumn = sortColumn;
                _sortDirection = sortDirections;
            }

            public int Compare(TItemType? x, TItemType? y)
            {
                var property = typeof(TItemType).GetProperty(_sortColumn);

                var xValue = property.GetValue(x);
                var yValue = property.GetValue(y);

                int result = 0;

                if (property.PropertyType == typeof(decimal))
                {
                    result = decimal.Compare((decimal)xValue, (decimal)yValue);
                }

                if (property.PropertyType == typeof(string))
                {
                    result = string.Compare((string)xValue, (string)yValue);
                }

                if (property.PropertyType == typeof(DateTime))
                {
                    result = DateTime.Compare((DateTime)xValue, (DateTime)yValue);
                }

                if (property.PropertyType == typeof(TimeSpan))
                {
                    result = TimeSpan.Compare((TimeSpan)xValue, (TimeSpan)yValue);
                }

                if (property.PropertyType == typeof(short))
                {
                    if ((short)xValue > (short)yValue)
                        result = 1;

                    if ((short)xValue < (short)yValue)
                        result = -1;
                }

                if (property.PropertyType == typeof(int))
                {
                    if ((int)xValue > (int)yValue)
                        result = 1;

                    if ((int)xValue < (int)yValue)
                        result = -1;
                }

                if (property.PropertyType == typeof(long))
                {
                    if ((long)xValue > (long)yValue)
                        result = 1;

                    if ((long)xValue < (long)yValue)
                        result = -1;
                }

                if (_sortDirection == SortDirections.Descending)
                    result *= -1;

                return result;
            }
        }

        private bool _shouldRender;

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

        //protected override void OnParametersSet()
        //{
        //}
    }
}