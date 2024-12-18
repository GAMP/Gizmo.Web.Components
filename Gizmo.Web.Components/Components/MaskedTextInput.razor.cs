namespace Gizmo.Web.Components
{
    public partial class MaskedTextInput<TValue> : MaskedNumericInputBase<TValue>
    {
        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-masked-text-input")
                 .Add("giz-text-input")
                 .If("giz-text-input--full-width", () => IsFullWidth)
                 .Add(Class)
                 .AsString();

        #endregion

    }
}
