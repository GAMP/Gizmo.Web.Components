using System.ComponentModel;

namespace Gizmo.Web.Components
{
    /// <summary>
    /// Visibilities.
    /// </summary>
    public enum Visibilities
    {
        Default,

        [Description("visible")]
        Visible,

        [Description("hidden")]
        Hidden,

        [Description("collapse")]
        Collapse
    }
}
