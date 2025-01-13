using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SortableListStartup.Shared.Demos;

public partial class GizmoNewList : ComponentBase
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    private readonly List<Item> _items = Enumerable.Range(1, 10).Select(i => new Item { Id = i, Name = $"Item {i}", DisplayOrder = i }).ToList();

    private async Task OnUpdateHandler(SortableListUpdateEventArgs args)
    {
        int draggedItemIdOrPreviousDisplayOrder = int.Parse(args.DraggedItemIdOrPreviousDisplayOrder); //I need the Id or the display order of the dragged item so I can identify it in the list.
        int newDisplayOrder = int.Parse(args.NewDisplayOrder); //I need the new display order so I can update the display order of all item in between the old location and the new location.

        var draggedItem = _items.Where(a => a.Id == draggedItemIdOrPreviousDisplayOrder).FirstOrDefault();

        if (draggedItem != null)
        {
            if (draggedItem.DisplayOrder < newDisplayOrder)
            {
                //The item was moved downwards.
                foreach (var item in _items.Where(a => a.DisplayOrder > draggedItem.DisplayOrder && a.DisplayOrder <= newDisplayOrder))
                {
                    item.DisplayOrder -= 1;
                }

                draggedItem.DisplayOrder = newDisplayOrder;
            }
            else
            {
                //The item was moved upwards.
                foreach (var item in _items.Where(a => a.DisplayOrder >= newDisplayOrder && a.DisplayOrder < draggedItem.DisplayOrder))
                {
                    item.DisplayOrder += 1;
                }

                draggedItem.DisplayOrder = newDisplayOrder;
            }

            await InvokeAsync(StateHasChanged);
        }
    }
}
