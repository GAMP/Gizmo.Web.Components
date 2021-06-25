using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace Gizmo.Web.Components
{
    public partial class CircularProgressBar
    {
        public enum CircularProgressBarSizes
        {
            [Description("small")]
            Small,

            [Description("medium")]
            Medium,

            [Description("large")]
            Large
        }

        #region CONSTRUCTOR
        public CircularProgressBar()
        {
        }
        #endregion

        [Parameter]
        public decimal Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                if (_value > 50)
                {
                    _left = 180;
                    _right = (_value - 50) / 50 * 180;
                }
                else
                {
                    _left = _value / 50 * 180;
                    _right = 0;
                }
            }
        }

        private decimal _value;
        private decimal _left;
        private decimal _right;

        [Parameter]
        public string Color { get; set; }

        [Parameter]
        public CircularProgressBarSizes Size { get; set; } = CircularProgressBarSizes.Medium;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string ClassName => new ClassMapper()
                 .Add("giz-circle-progress")
                 .Add($"giz-circle-progress--{Size.ToDescriptionString()}")
                 .AsString();

    }
}
