using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Gizmo.Web.Components
{
    public partial class PercentageStackBar
    {
        [Parameter]
        public List<decimal> Values { get; set; }

        [Parameter]
        public List<string> Colors { get; set; }

        protected string ClassName => new ClassMapper()
                 .Add("giz-percentage-stack-bar")
                 .AsString();

    }
}
