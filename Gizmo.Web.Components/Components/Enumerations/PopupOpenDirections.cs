using System.ComponentModel;

namespace Gizmo.Web.Components
{
    /// <summary>
    /// Popup open directions.
    /// </summary>
    public enum PopupOpenDirections
    {
        [Description("")]
        None,

        [Description("bottom")]
        Bottom,

        [Description("top")]
        Top,

        [Description("cursor")]
        Cursor
    }
}
