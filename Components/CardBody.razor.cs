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

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-card-body")
                 .AsString();

        #endregion

    }
}