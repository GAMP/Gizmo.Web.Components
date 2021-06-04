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
        public EventCallback<bool> IsCheckedChanged
        {
            get; set;
        }

        [Parameter]
        public bool IsDisabled
        {
            get; set;
        }

        #endregion

        protected Task OnChangeHandler(ChangeEventArgs args)
        {
            IsChecked = (bool)args.Value;
            return IsCheckedChanged.InvokeAsync(IsChecked);
        }
    }
}