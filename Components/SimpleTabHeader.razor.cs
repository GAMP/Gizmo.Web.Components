using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class SimpleTabHeader : CustomDOMComponentBase
    {
        private SimpleTabItem _selectedItem;

        [CascadingParameter]
        protected SimpleTab Parent { get; set; }

        #region PROPERTIES

        [Parameter]
        public List<SimpleTabItem> Items { get; set; }

        #endregion

        #region EVENTS

        protected void OnClickEvent(SimpleTabItem item)
        {
            Parent?.SetSelectedItem(item);
        }

        #endregion

        internal void SetSelectedItem(SimpleTabItem item)
        {
            _selectedItem = item;

            StateHasChanged();
        }

        string GetTabItemClass(SimpleTabItem item)
        {
            var itemClassName = new ClassMapper()
             .If("giz-simple-tab-active", () => item == _selectedItem)
             .If("giz-simple-tab-hidden", () => !item.IsVisible)
             .If("giz-simple-tab-disabled", () => item.IsDisabled).AsString();
            return itemClassName;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

    }
}