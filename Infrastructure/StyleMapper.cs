using System.Linq;

namespace Gizmo.Web.Components.Infrastructure
{
    /// <summary>
    /// Style mapper.
    /// </summary>
    public sealed class StyleMapper : BaseMapper
    {
        #region OVERRIDES
        
        public override string AsString()
        {
            return string.Join("; ", Items.Select(i => i()).Where(i => i != null));
        } 

        #endregion
    }
}
