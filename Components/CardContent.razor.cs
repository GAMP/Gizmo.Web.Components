using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class CardContent : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public CardContent()
        {
        }
        #endregion

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string ClassName => new ClassMapper()
                 .Add("g-card-content")
                 .AsString();
    }
}