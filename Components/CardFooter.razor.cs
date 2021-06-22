using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class CardFooter : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public CardFooter()
        {
        }
        #endregion

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string ClassName => new ClassMapper()
                 .Add("giz-card-footer")
                 .AsString();
    }
}