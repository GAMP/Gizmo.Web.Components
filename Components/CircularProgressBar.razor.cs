﻿using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class CircularProgressBar
    {
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
        public RenderFragment ChildContent { get; set; }

        protected string ClassName => new ClassMapper()
                 .Add("giz-circle-progress")
                 .AsString();

    }
}
