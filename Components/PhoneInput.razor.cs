using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class PhoneInput<TValue> : MaskedNumericInputBase<TValue>
    {
        #region CONSTRUCTOR
        public PhoneInput()
        {
        }
        #endregion

        #region FIELDS

        private List _popupContent;

        private bool _isOpen;
        private double _popupX;
        private double _popupY;
        private double _popupWidth;
        private IconSelectItem _selectedCountry;

        #endregion

        #region PROPERTIES

        [Parameter]
        public List<IconSelectItem> Countries { get; set; }

        [Parameter]
        public IconSelectItem SelectedCountry
        {
            get
            {
                return _selectedCountry;
            }
            set
            {
                if (_selectedCountry == value)
                    return;

                _selectedCountry = value;
                _ = SelectedCountryChanged.InvokeAsync(_selectedCountry);
            }
        }

        [Parameter]
        public EventCallback<IconSelectItem> SelectedCountryChanged { get; set; }

        [Parameter]
        public PopupOpenDirections OpenDirection { get; set; } = PopupOpenDirections.Bottom;

        [Parameter]
        public string MaximumHeight { get; set; }

        #endregion

        #region EVENTS

        protected async Task OnClickDropDownButtonHandler()
        {
            //await OnClickDropDownButton.InvokeAsync();

            if (!IsDisabled)
            {
                if (!_isOpen)
                    await Open();
                else
                    _isOpen = false;
            }
        }

        protected void SetSelectedCountry(int id)
        {
            SelectedCountry = Countries.Where(a => a.Id == id).FirstOrDefault();
            _isOpen = false;
        }

        #endregion

        #region METHODS

        private async Task Open()
        {
            if (OpenDirection == PopupOpenDirections.Cursor)
            {
                var windowSize = await JsInvokeAsync<WindowSize>("getWindowSize");
                var popupContentSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", _popupContent.Ref);

                var inputSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", Ref);

                _popupX = inputSize.Left;
                _popupWidth = inputSize.Width;

                if (inputSize.Bottom + popupContentSize.Height > windowSize.Height)
                {
                    _popupY = windowSize.Height - popupContentSize.Height;
                }
                else
                {
                    _popupY = inputSize.Bottom;
                }
            }

            int activeItemIndex = _popupContent.GetSelectedItemIndex();
            await _popupContent.SetActiveItemIndex(activeItemIndex);

            _isOpen = true;
        }

        #endregion

        #region OVERRIDES

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-masked-phone-input")
                 .Add($"giz-masked-phone-input--{Size.ToDescriptionString()}")
                 .If("giz-masked-phone-input--full-width", () => IsFullWidth)
                 .If("giz-masked-phone-input--open", () => _isOpen)
                 .Add(Class)
                 .AsString();

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-masked-phone-input__dropdown")
                 .If("giz-masked-phone-input__dropdown--cursor", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If("giz-masked-phone-input__dropdown--full-width", () => OpenDirection != PopupOpenDirections.Cursor)
                 .AsString();

        protected string PopupStyleValue => new StyleMapper()
                 .If($"top: {_popupY.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If($"left: {_popupX.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .If($"min-width: {_popupWidth.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => OpenDirection == PopupOpenDirections.Cursor)
                 .AsString();

        #endregion
    }
}
