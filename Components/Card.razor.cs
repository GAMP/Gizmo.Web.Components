using Gizmo.Web.Components.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class Card : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Card()
        {
        }
        #endregion

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string BackgroundColor { get; set; }

        [Parameter]
        public int ShadowLevel { get; set; } = 8;

        [Parameter]
        public int CornerRadius { get; set; } = 4;

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                ClassMapper
                   .Add("g-card")
                   .Add($"g-shadow-{ShadowLevel}");

                StyleMapper
                   .Add($"background-color: {BackgroundColor}")
                   .Add($"border-radius: {CornerRadius}px");

                StateHasChanged();
            }
        }
    }
}
