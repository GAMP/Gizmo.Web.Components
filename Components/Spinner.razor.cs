namespace Gizmo.Web.Components
{
    public partial class Spinner : CustomDOMComponentBase
    {
        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-animate-spinner")
                 .AsString();

        #endregion
    }
}
