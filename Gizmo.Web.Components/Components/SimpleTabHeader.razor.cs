using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class SimpleTabHeader : CustomDOMComponentBase
    {
        private int _selectedIndex;

        [CascadingParameter]
        protected SimpleTab Parent { get; set; }

        #region PROPERTIES

        [Parameter]
        public List<SimpleTabItem> Items { get; set; }

        #endregion

        #region EVENTS

        protected void OnClickEvent(int index)
        {
            Parent?.SetSelectedIndex(index);
        }

        #endregion

        internal void SetSelectedIndex(int index)
        {
            _selectedIndex = index;

            StateHasChanged();
        }

        string GetTabItemClass(int index)
        {
            var itemClassName = new ClassMapper()
             .If("giz-simple-tab--hidden", () => !Items[index].IsVisible)
             .If("active", () => index == _selectedIndex)
             .If("disabled", () => Items[index].IsDisabled)
             .AsString();
            return itemClassName;
        }
    }
}