using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class ButtonGroup : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public ButtonGroup()
        {
        }
        #endregion

        #region MEMBERS

        private HashSet<Button> _items = new HashSet<Button>();
        private Button _selectedItem;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Whether the button group is disabled.
        /// </summary>
        [Parameter]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Whether is mandatory to have an item selected.
        /// </summary>
        [Parameter]
        public bool IsMandatory { get; set; }

        /// <summary>
        /// The selection mode of the button group.
        /// </summary>
        [Parameter]
        public SelectionMode SelectionMode { get; set; } = SelectionMode.Single;

        /// <summary>
        /// The selected button in the button group.
        /// </summary>
        [Parameter]
        public Button SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;

                SetSelectedItem(value);
            }
        }

        [Parameter]
        public EventCallback<Button> SelectedItemChanged { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        #endregion

        internal void SetSelectedItem(Button item)
        {
            if (IsDisabled)
                return;

            //In single selection mode
            if (SelectionMode == SelectionMode.Single)
            {
                //If is mandatory
                if (IsMandatory)
                {
                    //Ignore null and same button clicks.
                    if (_selectedItem == item || item == null)
                        return;

                    //If button is different then set it as selected.
                    _selectedItem = item;
                }
                else //If is not mandatory
                {
                    //If same button the set null as selected.
                    if (_selectedItem == item)
                    {
                        _selectedItem = null;
                    }
                    else //If button is different then set it as selected.
                    {
                        _selectedItem = item;
                    }
                }

                _ = SelectedItemChanged.InvokeAsync(_selectedItem);

                //Update button states.
                foreach (var button in _items.ToArray())
                {
                    button.SetSelected(_selectedItem == button);
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

                    if (item.GetSelected())
                    {
                        //If the item is the only one selected then ignore.
                        if (firstSelected == null)
                            return;

                        _selectedItem = firstSelected;

                        //Update button state.
                        item.SetSelected(false);
                    }
                    else
                    {
                        _selectedItem = item;

                        //Update button state.
                        item.SetSelected(true);
                    }

                }
                else //If is not mandatory
                {
                    if (item.GetSelected())
                    {
                        //If same button the set the first available as selected.
                        if (_selectedItem == item)
                        {
                            _selectedItem = firstSelected;
                        }
                    }
                    else
                    {
                        _selectedItem = item;
                    }

                    //Toggle button state.
                    item.SetSelected(!item.GetSelected());
                }

                _ = SelectedItemChanged.InvokeAsync(_selectedItem);
            }
        }

        internal void Register(Button item)
        {
            _items.Add(item);
        }

        internal void Unregister(Button item)
        {
            _items.Remove(item);
        }

        protected string ClassName => new ClassMapper()
                 .Add("g-button-group")
                 .If("disabled", () => IsDisabled)
                 .AsString();

        protected override Task OnFirstAfterRenderAsync()
        {
            //If is mandatory and there is no item selected, select the first available item if any.
            if (IsMandatory && SelectedItem == null)
            {
                var firstItem = _items.FirstOrDefault();

                if (firstItem != null)
                {
                    SetSelectedItem(firstItem);
                }
            }

            return base.OnFirstAfterRenderAsync();
        }
    }
}