using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Gizmo.Web.Components.GizInput;

namespace Gizmo.Web.Components
{
    public partial class Select<TItemType> : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Select()
        {
        }
        #endregion

        #region MEMBERS
        private Dictionary<TItemType, SelectItem<TItemType>> _items = new Dictionary<TItemType, SelectItem<TItemType>>();
        private TItemType _value;
        private SelectItem<TItemType> _selectedItem;
        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public TItemType Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                if (_items.ContainsKey(_value))
                    SetSelectedItem(_items[_value]);
            }
        }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

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

        [Parameter]
        public EventCallback<TItemType> ValueChanged { get; set; }

        #endregion

        #region METHODS

        internal Task SetSelectedItem(SelectItem<TItemType> item)
        {
            IsOpen = false;

            if (_selectedItem != item)
            {
                _selectedItem = item;
                StateHasChanged();
            }

            return SetSelectedValue(item.Value);
        }

        internal Task SetSelectedValue(TItemType value)
        {
            if (!EqualityComparer<TItemType>.Default.Equals(_value, value))
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

        protected void OnClickMenuHandler(MouseEventArgs args)
        {
            IsOpen = true;
        }

        protected void OnClickOverlayHandler(MouseEventArgs args)
        {
            IsOpen = false;
        }
        #endregion

        internal void Register(SelectItem<TItemType> item)
        {
            _items[item.Value] = item;
        }

        internal void Unregister(SelectItem<TItemType> item)
        {
            _items.Remove(item.Value);
        }
        protected override Task OnFirstAfterRenderAsync()
        {
            if (_value != null)
            {
                if (_items.ContainsKey(_value))
                    SetSelectedItem(_items[_value]);
            }

            return base.OnFirstAfterRenderAsync();
        }

        protected string ClassName => new ClassMapper()
                 .Add("gizmo-select")
                 .If("giz-select-root--disabled", () => IsDisabled)
                 .If("giz-select-root--offset", () => OffsetY)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("gizmo-select-container")
                 .Add("g-popup-bottom")
                 .Add("g-shadow-8")
                 .AsString();
    }
}