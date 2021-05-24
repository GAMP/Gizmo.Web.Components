using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

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

        protected string ClassName => new ClassMapper()
                 .Add("g-select")
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("g-select-container")
                 .Add("g-popup-bottom")
                 .Add("g-shadow-8")
                 .AsString();

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
    }
}