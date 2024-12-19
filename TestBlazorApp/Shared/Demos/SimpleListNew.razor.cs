using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace TestBlazorApp.Shared.Demos;

public partial class SimpleListNew : ComponentBase
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    private readonly List<Item> _items = Enumerable.Range(1, 10).Select(i => new Item { Id = i, Name = $"Item {i}" }).ToList();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("import", "./Shared/Demos/SimpleListNew.razor.js");
        }
    }
}