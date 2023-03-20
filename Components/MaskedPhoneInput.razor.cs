using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class MaskedPhoneInput<TValue> : MaskedNumericInputBase<TValue>
    {
        #region CONSTRUCTOR
        public MaskedPhoneInput()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public string Prefix { get; set; }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-masked-phone-input")
                 .Add("giz-input-text")
                 .If("giz-input-text--full-width", () => IsFullWidth)
                 .Add(Class)
                 .AsString();

        #endregion

    }
}
