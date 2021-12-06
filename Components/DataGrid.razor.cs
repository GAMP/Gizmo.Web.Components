﻿using Microsoft.AspNetCore.Components;
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
        }
        #endregion

        #region FIELDS

        private readonly HashSet<DataGridColumn<TItemType>> _columns = new(Enumerable.Empty<DataGridColumn<TItemType>>());
        private ICollection<TItemType> _selectedItems = new HashSet<TItemType>();
        internal Dictionary<TItemType, DataGridRow<TItemType>> _rows = new Dictionary<TItemType, DataGridRow<TItemType>>();

        private bool _hasSelectedItems;
        private bool _hasSelectedAllItems;
        private int _providerTotalItems = 0;

        private ICollection<TItemType> _itemSource;
        private TItemType _selectedItem;
        private RenderFragment _childContent;

        #region CONTEXT MENU

        private double _clientX;
        private double _clientY;
        private Menu _contextMenu;

        #endregion

        #region SORTING

        private DataGridColumn<TItemType> _sortColumn;
        private SortDirections _sortDirection;
        private TItemType _activeItem;

        #endregion

        #region PERFORMANCE

        private bool _shouldRender;
        private ICollection<TItemType> _previousItemSource;
        private TItemType _previousSelectedItem;

        #endregion

        #region CRUD

        private bool _newRow = false;
        private TItemType _editedRow = default(TItemType);

        #endregion

        #endregion

        #region PROPERTIES

        [Parameter]
        public DataGridVariants Variant { get; set; } = DataGridVariants.Default;

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
            }
        }

        /// <summary>
        /// Gets or sets selected item.
        /// </summary>
        [Parameter]
        public TItemType SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (EqualityComparer<TItemType>.Default.Equals(_selectedItem, value))
                    return;

                _selectedItem = value;

                if (SelectionMode == SelectionMode.Single && SelectedItems?.Contains(_selectedItem) == false)
                {
                    SelectedItems?.Clear();
                    SelectedItems?.Add(_selectedItem);
                }
            }
        }

        [Parameter]
        public RenderFragment ChildContent
        {
            get
            {
                return _childContent;
            }
            set
            {
                if (_childContent == value)
                    return;

                _childContent = value;

                this.Refresh();
            }
        }

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

        [Parameter]
        public bool AllowCreate { get; set; }

        [Parameter]
        public bool AllowUpdate { get; set; }

        [Parameter]
        public bool AllowDelete { get; set; }

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

                this.Refresh();
            }

            return ValueTask.CompletedTask;
        }

        #endregion

        #region METHODS

        #region CRUD

        public void CreateRow()
        {
            TItemType newRow = (TItemType)Activator.CreateInstance(typeof(TItemType));
            CreateRow(newRow);
        }

        public void CreateRow(TItemType item)
        {
            if (!AllowCreate)
                return;

            //If there is already a row in edit mode then ignore.
            if (_editedRow != null)
                return;

            //If item is null then ignore it.
            if (EqualityComparer<TItemType>.Default.Equals(item, default(TItemType)))
                return;

            //Store the new row so we can set it in edit mode when it's added to the DataGrid.
            _newRow = true;
            _editedRow = item;

            if (IsVirtualized)
            {
                //TODO: Virtualization
            }
            else
            {
                ItemSource.Add(_editedRow);
            }

            //We need to refresh the DataGrid to see the new row.
            this.Refresh();
        }

        public void UpdateRow(TItemType item)
        {
            if (!AllowUpdate)
                return;

            //If there is already a row in edit mode then ignore.
            if (_editedRow != null)
                return;

            //If item is null then ignore it.
            if (EqualityComparer<TItemType>.Default.Equals(item, default(TItemType)))
                return;

            _newRow = false;
            _editedRow = item;

            if (_rows.ContainsKey(_editedRow))
            {
                _rows[_editedRow].SetEditMode(true);
            }
        }

        #endregion

        private void ExitEditMode()
        {
            //If the selected row is not the edited row.
            if (_editedRow != null)
            {
                //TODO: VALIDATE BEFORE EXIT EDIT MODE
                if (_rows.ContainsKey(_editedRow))
                {
                    _rows[_editedRow].SetEditMode(false);
                }

                _newRow = false;
                _editedRow = default(TItemType);
            }
        }

        public void Refresh()
        {
            _shouldRender = true;

            foreach (var row in _rows)
            {
                row.Value.Refresh();
            }

            StateHasChanged();
        }

        internal Task SetActiveItem(TItemType item)
        {
            ActiveItem = item;

            return Task.CompletedTask;
        }

        internal async Task OpenContextMenu(double clientX, double clientY)
        {
            var windowSize = await JsInvokeAsync<WindowSize>("getWindowSize");
            var contextMenuSize = await _contextMenu.GetListBoundingClientRect();

            //var gridPosition = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", Ref);
            //_clientX = clientX - gridPosition.Left;
            //if (gridPosition.Left + _clientX + contextMenuSize.Width > windowSize.Width)
            //{
            //    _clientX = windowSize.Width - gridPosition.Left - contextMenuSize.Width - 40;
            //}

            //_clientY = clientY - gridPosition.Top;

            if (clientX > windowSize.Width / 2)
            {
                //Open direction right to left.
                _clientX = clientX - contextMenuSize.Width;
                _contextMenu.SetDirection(ListDirections.Left);
            }
            else
            {
                _clientX = clientX;
                _contextMenu.SetDirection(ListDirections.Right);
            }

            if (clientY > windowSize.Height / 2)
            {
                //Open direction bottom to top.
                _clientY = clientY - contextMenuSize.Height;
                _contextMenu.ExpandBottomToTop = true;
            }
            else
            {
                _clientY = clientY;
                _contextMenu.ExpandBottomToTop = false;
            }

            _contextMenu.Open(_clientX, _clientY);
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

            //InvokeVoidAsync("writeLine", $"Add row {item}");

            _rows[item] = row;

            if (SelectedItems.Contains(item))
                _rows[item].SetSelected(true);

            if (EqualityComparer<TItemType>.Default.Equals(item, _editedRow))
            {
                _rows[item].SetEditMode(true);
            }
        }

        internal void UpdateRow(DataGridRow<TItemType> row, TItemType item)
        {
            if (item == null)
                return;

            //InvokeVoidAsync("writeLine", $"Update row {item}");

            var actualRow = _rows.Where(a => a.Value == row).FirstOrDefault();
            if (!actualRow.Equals(default(KeyValuePair<DataGridRow<TItemType>, TItemType>)))
            {
                //InvokeVoidAsync("writeLine", $"Remove row {actualRow.Key}");
                _rows.Remove(actualRow.Key);
            }

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

            //InvokeVoidAsync("writeLine", $"Remove row {item}");
            //_rows.Remove(item);

            var actualRow = _rows.Where(a => a.Value == row).FirstOrDefault();
            if (!actualRow.Equals(default(KeyValuePair<DataGridRow<TItemType>, TItemType>)))
            {
                //InvokeVoidAsync("writeLine", $"Remove row {actualRow.Key}");
                _rows.Remove(actualRow.Key);
            }
        }

        internal async Task DoubleClickRow(DataGridRow<TItemType> item)
        {
            TItemType dataItem = item.Item;

            await OnDoubleClickItem.InvokeAsync(dataItem);
        }

        internal bool VerifySelected()
        {
            bool selectionUpdated = false;

            if (SelectedItems != null)
            {
                var previouslySelectedItems = SelectedItems.ToList();

                foreach (var item in previouslySelectedItems)
                {
                    if (_itemSource.Where(a => EqualityComparer<TItemType>.Default.Equals(a, item)).Count() == 0)
                    {
                        SelectedItems.Remove(item);
                        selectionUpdated = true;
                    }
                }
            }

            if (SelectedItems == null || SelectedItems.Count == 0)
            {
                _selectedItem = default;
            }

            return selectionUpdated;
        }

        internal async Task SelectRow(DataGridRow<TItemType> item, bool selected)
        {
            //Called once data row item is clicked, right clicked or row checkbox clicked.

            TItemType dataItem = item.Item;

            //No matter of selection the clicked item is always the selected one
            _selectedItem = dataItem;

            bool wasSelected = SelectedItems?.Contains(dataItem) == true;

            if (wasSelected == selected)
                return;

            if (SelectionMode == SelectionMode.Single)
            {
                //In single selection mode.

                //If current row is already selected.
                if (wasSelected)
                {
                    if (AllowUpdate)
                    {
                        _newRow = false;
                        _editedRow = _selectedItem;

                        if (_rows.ContainsKey(_editedRow))
                        {
                            _rows[_editedRow].SetEditMode(true);
                        }
                    }
                    else
                    {
                        //Clear selected items list and set selected property to false.
                        SelectedItems?.Clear();
                        item.SetSelected(false);
                    }
                }
                else
                {
                    ExitEditMode();

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

                if (SelectedItems == null || SelectedItems.Count == 0)
                {
                    _selectedItem = default;
                }
            }

            UpdateHeaderCheckbox();

            await SelectedItemChanged.InvokeAsync(_selectedItem);
            await SelectedItemsChanged.InvokeAsync();
        }

        private void UpdateHeaderCheckbox()
        {
            var previousHasSelectedItems = _hasSelectedItems;
            var previousHasSelectedAllItems = _hasSelectedAllItems;

            if (SelectionMode == SelectionMode.Single)
            {
                _hasSelectedItems = _selectedItem != null;
                _hasSelectedAllItems = _selectedItem != null && _itemSource.Count == 1;
            }
            else
            {
                if (SelectedItems?.Count > 0)
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
                else
                {
                    _hasSelectedItems = false;
                }
            }

            if (previousHasSelectedItems != _hasSelectedItems ||
                previousHasSelectedAllItems != _hasSelectedAllItems)
                this.Refresh();
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                _shouldRender = false;
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            if (SelectionMode == SelectionMode.Single)
            {
                if (_selectedItem != null && _rows.ContainsKey(_selectedItem))
                {
                    var selectedRow = _rows[_selectedItem];
                    selectedRow.SetSelected(true);

                    foreach (var row in _rows.Where(a => a.Value != selectedRow))
                    {
                        row.Value.SetSelected(false);
                    }
                }
                else
                {
                    //Deselect all rows.
                    foreach (var row in _rows)
                    {
                        row.Value.SetSelected(false);
                    }
                }
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnParametersSetAsync()
        {
            bool newItemSource = !EqualityComparer<ICollection<TItemType>>.Default.Equals(_previousItemSource, _itemSource);
            bool newSelectedItem = !EqualityComparer<TItemType>.Default.Equals(_previousSelectedItem, _selectedItem);

            _previousItemSource = _itemSource;
            _previousSelectedItem = _selectedItem;

            //In case of new item source make a full refresh.
            if (newItemSource)
            {
                VerifySelected();

                this.Refresh();
            }
            else
            {
                if (newSelectedItem)
                {
                    if (VerifySelected())
                        this.Refresh();
                }
            }

            await base.OnParametersSetAsync();
        }

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }
    }
}