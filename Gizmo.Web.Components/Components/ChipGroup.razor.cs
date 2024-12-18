using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class ChipGroup : CustomDOMComponentBase
    {
        #region FIELDS

        private HashSet<Chip> _items = new HashSet<Chip>();
        private Chip _selectedItem;
        private ICollection<Chip> _selectedItems = new HashSet<Chip>();

        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Whether the chip group is disabled.
        /// </summary>
        [Parameter]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Whether is mandatory to have an item selected.
        /// </summary>
        [Parameter]
        public bool IsMandatory { get; set; }

        /// <summary>
        /// The selection mode of the chip group.
        /// </summary>
        [Parameter]
        public SelectionMode SelectionMode { get; set; } = SelectionMode.Single;

        /// <summary>
        /// The selected chip in the chip group.
        /// </summary>
        [Parameter]
        public Chip SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;

                SelectItem(value, true);
            }
        }

        [Parameter]
        public EventCallback<Chip> SelectedItemChanged { get; set; }

        [Parameter]
        public EventCallback<ICollection<Chip>> SelectedItemsChanged { get; set; }

        [Parameter]
        public ICollection<Chip> SelectedItems
        {
            get { return _selectedItems; }
            set { _selectedItems = value; }
        }

        [Parameter]
        public ButtonColors Color { get; set; } = ButtonColors.Primary;

        #endregion

        #region METHODS

        internal void SelectItem(Chip item, bool selected)
        {
            bool wasSelected = SelectedItems?.Contains(item) == true;

            if (wasSelected == selected)
                return;

            //In single selection mode
            if (SelectionMode == SelectionMode.Single)
            {
                //If is mandatory
                if (IsMandatory)
                {
                    //Ignore null and same chip clicks.
                    if (_selectedItem == item || item == null)
                        return;

                    //If chip is different then set it as selected.
                    _selectedItem = item;

                    SelectedItems?.Clear();
                    SelectedItems?.Add(_selectedItem);
                }
                else //If is not mandatory
                {
                    //Clear selected items list.
                    SelectedItems?.Clear();

                    //If same chip the set null as selected.
                    if (_selectedItem == item)
                    {
                        _selectedItem = null;
                    }
                    else //If chip is different then set it as selected.
                    {
                        _selectedItem = item;
                        SelectedItems?.Add(_selectedItem);
                    }
                }

                _ = SelectedItemChanged.InvokeAsync(_selectedItem);
                _ = SelectedItemsChanged.InvokeAsync(_selectedItems);

                //Update chip states.
                foreach (var chip in _items.ToArray())
                {
                    chip.SetSelected(_selectedItem == chip);
                }
            }
            else //In extended selection mode
            {
                var firstSelected = _items.Where(a => a != item && a.GetSelected()).FirstOrDefault();

                //If is mandatory
                if (IsMandatory)
                {
                    //Ignore null.
                    if (item == null)
                        return;

                    if (wasSelected)
                    {
                        //If the item is the only one selected then ignore.
                        if (firstSelected == null)
                            return;

                        _selectedItem = firstSelected;

                        SelectedItems?.Remove(item);

                        //Update chip state.
                        item.SetSelected(false);
                    }
                    else
                    {
                        _selectedItem = item;

                        SelectedItems?.Add(_selectedItem);

                        //Update chip state.
                        item.SetSelected(true);
                    }
                }
                else //If is not mandatory
                {
                    if (wasSelected)
                    {
                        //If same chip the set the first available as selected.
                        if (_selectedItem == item)
                        {
                            _selectedItem = firstSelected;
                        }
                    }
                    else
                    {
                        _selectedItem = item;
                    }

                    //Set chip state.
                    item.SetSelected(selected);
                }

                _ = SelectedItemChanged.InvokeAsync(_selectedItem);
                _ = SelectedItemsChanged.InvokeAsync(_selectedItems);
            }
        }

        internal void Register(Chip item)
        {
            _items.Add(item);
        }

        internal void Unregister(Chip item)
        {
            _items.Remove(item);
        }

        #endregion

        #region OVERRIDES

        protected override Task OnFirstAfterRenderAsync()
        {
            //If is mandatory and there is no item selected, select the first available item if any.
            if (IsMandatory && SelectedItem == null)
            {
                var firstItem = _items.FirstOrDefault();

                if (firstItem != null)
                {
                    SelectItem(firstItem, true);
                }
            }

            return base.OnFirstAfterRenderAsync();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-chip-group")
                 .Add($"{Color.ToDescriptionString()}")
                 .If("disabled", () => IsDisabled)
                 .AsString();

        #endregion

    }
}