using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class SelectItem<TValue> : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public SelectItem()
        {
        }
        #endregion

        #region PROPERTIES

        internal ListItem ListItem { get; set; }

        [CascadingParameter]
        protected ISelect<TValue> Parent { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        #endregion

        #region EVENTS

        protected Task OnListItemClickHandler(MouseEventArgs args)
        {
            if (Parent != null)
            {
                return Parent.SetSelectedItem(this);
            }

            return Task.CompletedTask;
        }

        #endregion

        protected override void OnInitialized()
        {
            if (Parent != null)
            {
                Parent.Register(this);
            }
        }

        public override void Dispose()
        {
            try
            {
                if (Parent != null)
                {
                    Parent.Unregister(this);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }
    }
}