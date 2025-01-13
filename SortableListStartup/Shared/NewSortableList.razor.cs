using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace SortableListStartup.Shared
{
    public partial class NewSortableList<T>
    {
        private DotNetObjectReference<NewSortableList<T>>? _selfReference;

        [Parameter]
        public RenderFragment<T>? SortableItemTemplate { get; set; }

        [Parameter, AllowNull]
        public List<T> Items { get; set; }

        [Parameter]
        public EventCallback<SortableListUpdateEventArgs> OnUpdate { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _selfReference = DotNetObjectReference.Create(this);
                try
                {
                    await JsRuntime.InvokeVoidAsync("initDraggable", Id, _selfReference);
                }
                catch (Exception ex)
                {

                }
            }
        }

        public override void Dispose()
        {
            _selfReference?.Dispose();

            base.Dispose();
        }

        [JSInvokable]
        public Task HandleDragDrop(string draggedItemId, string targetItemId)
        {
            return OnUpdate.InvokeAsync(new SortableListUpdateEventArgs()
            {
                DraggedItemIdOrPreviousDisplayOrder = draggedItemId,
                NewDisplayOrder = targetItemId
            });
        }
    }
}
