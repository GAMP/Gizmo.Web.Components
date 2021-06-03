using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Select<TItemType> : CustomDOMComponentBase
    {
        public enum SelectSize
        {
            Normal = 0,
            Large = 1
        }

        #region CONSTRUCTOR
        public Select()
        {
        }
        #endregion

        #region MEMBERS
        private string _text;
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
        public SelectSize Size { get; set; } = SelectSize.Normal;

        [Parameter]
        public bool HasOutline { get; set; } = true;

        [Parameter]
        public bool HasShadow { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public EventCallback<TItemType> ValueChanged { get; set; }

        #endregion

        #region METHODS
        internal Task SetSelectedItem(SelectItem<TItemType> item)
        {
            IsOpen = false;

            _text = item.Text;

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
                 .If("giz-select-root--outline", () => HasOutline)
                 .If("giz-select-root--shadow", () => HasShadow)
                 .If("giz-select-root--full-width", () => IsFullWidth)
                 .If("giz-select-root--large", () => Size == SelectSize.Large)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("gizmo-select-container")
                 .Add("g-popup-bottom")
                 .Add("g-shadow-8")
                 .AsString();
    }
}