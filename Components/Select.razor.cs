using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Select<TValue> : GizInputBase<TValue>, ISelect<TValue>
    {
        #region CONSTRUCTOR
        public Select()
        {
        }
        #endregion

        #region FIELDS
        private Dictionary<TValue, SelectItem<TValue>> _items = new Dictionary<TValue, SelectItem<TValue>>();
        private TValue _value;
        private SelectItem<TValue> _selectedItem;
        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public TValue Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (EqualityComparer<TValue>.Default.Equals(_value, value))
                    return;

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
        public string MaximumHeight { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public bool OffsetY { get; set; }

        [Parameter]
        public InputSizes Size { get; set; } = InputSizes.Normal;

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

        protected Task OnClickInputHandler(MouseEventArgs args)
        {
            if (!IsDisabled)
                IsOpen = true;

            return Task.CompletedTask;
        }

        protected Task OnClickOverlayHandler(MouseEventArgs args)
        {
            IsOpen = false;

            return Task.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override Task OnFirstAfterRenderAsync()
        {
            if (_value != null)
            {
                if (_items.ContainsKey(_value))
                    SetSelectedItem(_items[_value]);
            }

            return base.OnFirstAfterRenderAsync();
        }

        #endregion

        #region METHODS

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
            }

            StateHasChanged();

            if (selectItem != null)
                return SetSelectedValue(selectItem.Value);
            else
                return SetSelectedValue(default(TValue));
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-input-select")
                 //.If("giz-select-root--disabled", () => IsDisabled)
                 //.If("giz-select-root--offset", () => OffsetY)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-input-select-dropdown-menu")
                 .Add("giz-select-dropdown-full-width")
                 .Add("giz-popup-bottom")
                 .AsString();

        #endregion

    }
}