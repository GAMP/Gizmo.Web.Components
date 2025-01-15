using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SortableListStartup.Shared.Demos;

public partial class GizmoNewList : ComponentBase
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;

    private readonly List<Item> _items = Enumerable.Range(1, 10).Select(i => new Item { Id = i, Name = $"Item {i}", DisplayOrder = i })
        .ToList();

    private async Task OnUpdateHandler(SortableListUpdateEventArgs args)
    {
        int draggedItemIdOrPreviousDisplayOrder =
            int.Parse(args.DraggedItemId); //I need the Id or the display order of the dragged item so I can identify it in the list.
        int newDisplayOrder =
            int.Parse(args.TargetItemId); //I need the new display order so I can update the display order of all item in between the old location and the new location.

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

    public async Task OnUpdateHandler2(SortableListUpdateEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(args.DraggedItemId))
            throw new ArgumentNullException(nameof(args.DraggedItemId));

        if (string.IsNullOrWhiteSpace(args.TargetItemId))
            throw new ArgumentNullException(nameof(args.TargetItemId));

        var draggedItem =
            _items.SingleOrDefault(i => i.Id.ToString() == args.DraggedItemId)
            ?? throw new InvalidOperationException($"Dragged item with ID {args.DraggedItemId} not found.");

        var targetItem =
            _items.SingleOrDefault(i => i.Id.ToString() == args.TargetItemId)
            ?? throw new InvalidOperationException($"Target item with ID {args.TargetItemId} not found.");

        Console.WriteLine($"Items before:\n{string.Join("\n", _items.Select(i => $"Id: {i.Id} - DisplayOrder: {i.DisplayOrder}"))}");

        _items.Remove(draggedItem);
        _items.Insert(_items.IndexOf(targetItem) + args.TargetItemOffset, draggedItem);

        var minDisplayOrder = _items.Min(i => i.DisplayOrder);

        for (var i = 0; i < _items.Count; i++)
            _items[i].DisplayOrder = minDisplayOrder + i;

        Console.WriteLine($"Items after:\n{string.Join("\n", _items.Select(i => $"Id: {i.Id} - DisplayOrder: {i.DisplayOrder}"))}");

        await InvokeAsync(StateHasChanged);
    }
}