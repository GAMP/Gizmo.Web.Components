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

        private readonly HashSet<DataGridColumn<TItemType>> _columns = new (Enumerable.Empty<DataGridColumn<TItemType>>());
        private ICollection<TItemType> _selectedItems = new HashSet<TItemType>();

        #endregion

        #region PROPERTIES

        #region PUBLIC

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
        public int OverscanCount
        {
            get; set;
        }

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

        #region INTERNAL

        internal void AddColumn(DataGridColumn<TItemType> column)
        {
            _columns.Add(column);
        }

        internal void RemoveColumn(DataGridColumn<TItemType> column)
        {
            _columns.Remove(column);
        }

        internal ValueTask OnHeaderRowMouseEvent(MouseEventArgs args, DataGridColumn<TItemType> column)
        {
            return ValueTask.CompletedTask;
        }

        internal async ValueTask OnDataRowMouseEvent(MouseEventArgs args, TItemType dataItem)
        {
            //called once data row item is clicked

            //no matter of selection the clicked item is always the selected one
            SelectedItem = dataItem;

            bool wasSelected = SelectedItems?.Contains(dataItem) == true;

            if (SelectionMode == SelectionMode.Single)
            {
                SelectedItems?.Clear();
                if (!wasSelected && SelectedItems?.Contains(dataItem) == false)
                {
                    SelectedItems?.Add(dataItem);
                }             
            }
            else if (SelectionMode == SelectionMode.Extended)
            {
                if (SelectedItems?.Contains(dataItem) == false)
                {
                    SelectedItems.Add(dataItem);
                }
                else if(SelectedItems?.Contains(dataItem) == true)
                {
                    SelectedItems.Remove(dataItem);                   
                }
            }         

            if(SelectedItems?.Count == 0)
            {
                SelectedItem = default;
            }
            
            await SelectedItemChanged.InvokeAsync(dataItem);
            await SelectedItemsChanged.InvokeAsync();
        }

        internal ValueTask OnDataCellMouseEvent(MouseEventArgs args, TItemType dataItem,DataGridColumn<TItemType> column)
        {
            //called once cell data item is clicked

            return ValueTask.CompletedTask;
        }

        #endregion
    }
}
