using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
        private SelectItem<TItemType> _selectedItem;
        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public TItemType Value { get; set; }

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

            _selectedItem = item;

            return SetSelectedValue(item.Value);
        }

        internal Task SetSelectedValue(TItemType value)
        {
            Value = value;

            return ValueChanged.InvokeAsync(Value);
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