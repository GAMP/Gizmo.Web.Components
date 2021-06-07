using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Gizmo.Web.Components.GizInput;

namespace Gizmo.Web.Components
{
    public partial class AutoComplete<TItemType, TValue> : InputBase<TValue>, ISelect<TValue>
    {
        #region CONSTRUCTOR
        public AutoComplete()
        {
        }
        #endregion

        #region MEMBERS
        private Dictionary<TValue, SelectItem<TValue>> _items = new Dictionary<TValue, SelectItem<TValue>>();
        private string _text;
        private TValue _value;
        private SelectItem<TValue> _selectedItem;
        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment<TItemType> Item { get; set; }

        [Parameter]
        public ICollection<TItemType> ItemSource
        {
            get; set;
        }

        [Parameter]
        public TValue Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                if (_value != null)
                {
                    if (_items.ContainsKey(_value))
                        SetSelectedItem(_items[_value]);
                }
                else
                {
                    SetSelectedItem(null);
                }
            }
        }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public int MaximumHeight { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public bool OffsetY { get; set; }

        [Parameter]
        public InputSize Size { get; set; } = InputSize.Normal;

        [Parameter]
        public bool HasOutline { get; set; } = true;

        [Parameter]
        public bool HasShadow { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        #endregion

        #region METHODS

        internal Task SetSelectedValue(TValue value)
        {
            if (!EqualityComparer<TValue>.Default.Equals(_value, value))
            {
                _value = value;
                return ValueChanged.InvokeAsync(_value);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        #endregion

        #region EVENTS

        protected Task OnInputKeyDown(KeyboardEventArgs args)
        {
            if (!IsOpen)
                IsOpen = true;

            return Task.CompletedTask;
        }

        protected Task OnChangeHandler(ChangeEventArgs args)
        {
            var newValue = args?.Value as string;

            return Task.CompletedTask;
        }

        protected Task OnClickHandler(MouseEventArgs args)
        {
            return Task.CompletedTask;
        }

        protected void OnClickMenuHandler(MouseEventArgs args)
        {
            IsOpen = true;
        }

        protected void OnClickOverlayHandler(MouseEventArgs args)
        {
            IsOpen = false;
        }

        #endregion

        protected override Task OnFirstAfterRenderAsync()
        {
            if (_value != null)
            {
                if (_items.ContainsKey(_value))
                    SetSelectedItem(_items[_value]);
            }

            return base.OnFirstAfterRenderAsync();
        }

        public void Register(SelectItem<TValue> selectItem)
        {
            _items[selectItem.Value] = selectItem;
        }

        public void Unregister(SelectItem<TValue> selectItem)
        {
            _items.Remove(selectItem.Value);
        }

        public Task SetSelectedItem(SelectItem<TValue> selectItem)
        {
            IsOpen = false;

            if (_selectedItem != selectItem)
            {
                _selectedItem = selectItem;
                StateHasChanged();
            }

            if (selectItem != null)
                return SetSelectedValue(selectItem.Value);
            else
                return SetSelectedValue(default(TValue));
        }

        protected string ClassName => new ClassMapper()
                 .Add("giz-input-select")
                 //.If("giz-select-root--disabled", () => IsDisabled)
                 //.If("giz-select-root--offset", () => OffsetY)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-input-select-dropdown-menu")
                 .Add("giz-select-dropdown-full-width")
                 .Add("g-popup-bottom")
                 .Add("giz-elevation-2")
                 .AsString();
    }
}