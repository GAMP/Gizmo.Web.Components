namespace SortableListStartup.Shared
{
    public class SortableListUpdateEventArgs : EventArgs
    {
        public string DraggedItemId { get; init; } = string.Empty;
        public string TargetItemId { get; init; } = string.Empty;
        public int TargetItemOffset { get; init; }
    }
}