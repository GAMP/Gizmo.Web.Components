using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class CheckBox : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public CheckBox()
        {
            ClassMapper
                .Add("checkbox")
                .If("is-disabled", () => IsDisabled)
                .If("indeterminate", () => IsIndeterminate);
            
        } 
        #endregion

        #region FIELDS
        private bool _isTriState;
        private bool _isChecked;
        private bool _isIndeterminate;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets if checkbox is checked.
        /// </summary>
        [Parameter()]
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }

        [Parameter()]
        public bool IsDisabled
        {
            get;set;
        }

        /// <summary>
        /// Gets or sets if checkbox is tri state.
        /// </summary>
        [Parameter()]
        public bool IsTriState
        {
            get { return _isTriState; }
            set { _isTriState = value; }
        } 

        [Parameter()]
        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
            set { _isIndeterminate = value; }
        }

        #endregion

        /// <summary>
        /// Checkbox state change event callback.
        /// </summary>
        [Parameter()]
        public EventCallback<bool?> IsCheckedChangeCallback
        {
            get;set;
        }

        private async Task TrySetIndeterminateAsync(bool value)
        {
            await InvokeVoidAsync("window.jsinterop.setPropByElement", Ref, "indeterminate", value);
        }

        protected override async Task OnFirstAfterRenderAsync()
        {
            if (IsTriState && IsIndeterminate)
            {
                await TrySetIndeterminateAsync(true);
            }
        }

        protected  Task OnChange(ChangeEventArgs args)
        {
            return Task.CompletedTask;
        }

        protected async Task OnClickHandler(MouseEventArgs args)
        {
            if (!IsRendered)
                return;

            await Task.Delay(1000);
            IsChecked = false;
            StateHasChanged();
            //bool? newCheckedState = !IsChecked;

            //if(IsTriState)
            //{
            //    if (IsIndeterminate)
            //        newCheckedState = false;
            //    if (IsChecked)
            //        IsIndeterminate = true;

            //}

            //if (CheckStateChangeCallback.HasDelegate)
            //    await CheckStateChangeCallback.InvokeAsync(newCheckedState);

        }

    }
}
