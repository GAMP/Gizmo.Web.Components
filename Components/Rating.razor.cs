using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class Rating
    {
        private decimal _value;
        private int _selectedStar = 0;

        [Parameter]
        public decimal Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value == value)
                    return;

                _value = value;
                _ = ValueChanged.InvokeAsync(_value);
            }
        }

        [Parameter]
        public EventCallback<decimal> ValueChanged { get; set; }

        [Parameter]
        public string BackgroundColor { get; set; } = "#585d6f";

        [Parameter]
        public string Color { get; set; } = "#ffffff";

        [Parameter]
        public bool IsReadOnly { get; set; }

        protected void OnMouseOverStar(int index)
        {
            _selectedStar = index;
        }

        protected void OnMouseOut()
        {
            _selectedStar = 0;
        }

        protected void OnClickStar(int index)
        {
            Value = index;
        }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-rating")
                 .If("readonly", () => IsReadOnly)
                 .AsString();

        #endregion
    }
}