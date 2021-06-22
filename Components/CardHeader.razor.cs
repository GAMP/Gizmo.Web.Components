using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class CardHeader : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public CardHeader()
        {
        }
        #endregion

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string ClassName => new ClassMapper()
                 .Add("giz-card-header")
                 .AsString();
    }
}