using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class CardBody : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public CardBody()
        {
        }
        #endregion

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string ClassName => new ClassMapper()
                 .Add("g-card-body")
                 .AsString();
    }
}