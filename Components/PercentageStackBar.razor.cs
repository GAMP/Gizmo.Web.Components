using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Web.Components
{
    public partial class PercentageStackBar
    {
        #region CONSTRUCTOR
        public PercentageStackBar()
        {
        }
        #endregion

        #region FIELDS

        private List<decimal> _values;
        private decimal _total;
        private List<decimal> _percentValues = new List<decimal>();

        #endregion

        #region PROPERTIES

        [Parameter]
        public bool ShowValues { get; set; }

        [Parameter]
        public List<decimal> Values
        {
            get
            {
                return _values;
            }
            set
            {
                _values = value;

                if (_values != null)
                {
                    _total = _values.Sum(a => a);

                    _percentValues.Clear();
                    foreach (var val in _values)
                    {
                        if (_total == 0)
                        {
                            _percentValues.Add(0);
                        }
                        else
                        {
                            _percentValues.Add(val / _total * 100);
                        }
                    }
                }
                else
                {
                    _total = 0;
                }
            }
        }

        [Parameter]
        public List<string> Colors { get; set; }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-percentage-stack-bar")
                 .AsString();

        #endregion

    }
}
