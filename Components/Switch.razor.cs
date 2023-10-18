using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Switch : CustomDOMComponentBase
    {
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public bool IsChecked { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public EventCallback<bool> IsCheckedChanged { get; set; }

        [Parameter]
        public IconSizes Size { get; set; } = IconSizes.Medium;

        protected Task OnChangeHandler(ChangeEventArgs args)
        {
            if (IsDisabled)
                return Task.CompletedTask;

            IsChecked = (bool)args.Value;
            return IsCheckedChanged.InvokeAsync(IsChecked);
        }

        protected string ClassName => new ClassMapper()
            .Add("giz-switch")
            .Add($"giz-switch--{Size.ToDescriptionString()}")
            .If("disabled", () => IsDisabled)
            .AsString();
    }
}
