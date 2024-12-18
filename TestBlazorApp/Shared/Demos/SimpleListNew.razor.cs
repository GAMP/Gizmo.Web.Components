using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace TestBlazorApp.Shared.Demos;

public partial class SimpleListNew : ComponentBase
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("import", "./Shared/Demos/SimpleListNew.razor.js");
        }
    }
}