namespace SortableListStartup.Shared
{
    public class SortableListUpdateEventArgs : EventArgs
    {
        public string DraggedItemIdOrPreviousDisplayOrder { get; set; } = string.Empty;
        public string NewDisplayOrder { get; set; } = string.Empty;
    }
}
