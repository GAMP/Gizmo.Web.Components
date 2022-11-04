using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public partial class MaskedTextInput<TValue> : MaskedInputBase<TValue>
    {
        #region CONSTRUCTOR
        public MaskedTextInput()
        {
        }
        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-masked-text-input")
                 .Add("giz-input-text")
                 .If("giz-input-text--full-width", () => IsFullWidth)
                 .Add(Class)
                 .AsString();

        #endregion

    }
}