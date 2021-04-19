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
                .If("is-disabled", () => IsDisabled);
        }
        #endregion

        #region FIELDS
        private bool _isChecked;
        private bool _isTriState;
        private bool _isIndeterminate;
        private bool _nextIndeterminate = false;
        #endregion

        #region PROPERTIES

        [Parameter]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets if checkbox is checked.
        /// </summary>
        [Parameter()]
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

        [Parameter()]
        public bool IsDisabled
        {
            get; set;
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
            set
            {
                if (_isIndeterminate != value)
                {
                    _isIndeterminate = value;
                    StateHasChanged();

                    Task.Run(async () =>
                    {
                        await TrySetIndeterminateAsync(_isIndeterminate);
                    });
                }
            }
        }

        public bool PreventDefault
        {
            get
            {
                return _isIndeterminate;
            }
        }

        #endregion

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

        protected Task OnChangeHandler(ChangeEventArgs args)
        {
            var value = (bool)args.Value;

            if (IsChecked != value)
            {
                _nextIndeterminate = value;
                IsChecked = value;
                return IsCheckedChanged.InvokeAsync(IsChecked);
            }

            return Task.CompletedTask;
        }

        protected void OnClickHandler(MouseEventArgs args)
        {
            if (IsTriState)
            {
                if (IsIndeterminate)
                {
                    IsIndeterminate = false;
                }
                else
                {
                    if (_nextIndeterminate)
                    {
                        IsIndeterminate = true;
                    }
                }
            }
        }
    }
}