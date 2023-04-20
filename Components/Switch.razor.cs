using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class Switch : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Switch()
        {
        }
        #endregion

        #region FIELDS

        private bool _isChecked;

        #endregion

        #region PROPERTIES

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }

        [Parameter]
        public EventCallback<bool> IsCheckedChanged { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public Icons? ThumbSVGIcon { get; set; }

        #endregion

        #region EVENTS

        protected Task OnChangeHandler(ChangeEventArgs args)
        {
            if (IsDisabled)
                return Task.CompletedTask;

            IsChecked = (bool)args.Value;
            return IsCheckedChanged.InvokeAsync(IsChecked);
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-switch")
                 .If("disabled", () => IsDisabled)
                 .AsString();

        #endregion
    }
}
