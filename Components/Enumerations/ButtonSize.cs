using System.ComponentModel;

namespace Gizmo.Web.Components
{
    /// <summary>
    /// Button size.
    /// </summary>
    public enum ButtonSize
    {
        [Description("small")]
        Small = 0,

        [Description("medium")]
        Medium = 1,

        [Description("large")]
        Large = 2,

        [Description("extra-large")]
        ExtraLarge = 3
    }
}