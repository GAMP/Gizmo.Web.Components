using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    /// <summary>
    /// Tooltip for a chart, rendered as markup instead of being painted into the chart's canvas.
    /// </summary>
    /// <remarks>
    /// A charting library that draws its own tooltip draws it into the canvas bitmap, so on a small
    /// canvas the tooltip is clipped at the edge with no way to style around it. This component is
    /// ordinary DOM and has no such limit.
    /// <para>
    /// It carries no colours and no placement of its own: it publishes the caret position as the
    /// <c>--giz-chart-tooltip-x</c> and <c>--giz-chart-tooltip-y</c> custom properties and leaves
    /// the anchoring to the consuming stylesheet, because every chart has a different geometry to
    /// stay inside. Consumers are expected to style <c>giz-chart-tooltip</c> to their own theme.
    /// </para>
    /// </remarks>
    public partial class ChartTooltip : CustomDOMComponentBase
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or sets whether the tooltip is shown.
        /// </summary>
        [Parameter]
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the caret's horizontal position, in pixels, relative to the positioned
        /// ancestor the tooltip is rendered in.
        /// </summary>
        [Parameter]
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the caret's vertical position, in pixels, relative to the positioned
        /// ancestor the tooltip is rendered in.
        /// </summary>
        [Parameter]
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the lines shown as the tooltip's heading.
        /// </summary>
        [Parameter]
        public IReadOnlyList<string> TitleLines { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the lines shown as the tooltip's body.
        /// </summary>
        [Parameter]
        public IReadOnlyList<string> BodyLines { get; set; } = Array.Empty<string>();

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-chart-tooltip")
                 .If(Class, () => !string.IsNullOrEmpty(Class))
                 .AsString();

        protected string StyleValue => new StyleMapper()
                 .Add($"--giz-chart-tooltip-x: {X.ToString(CultureInfo.InvariantCulture)}px")
                 .Add($"--giz-chart-tooltip-y: {Y.ToString(CultureInfo.InvariantCulture)}px")
                 .If(Style, () => !string.IsNullOrEmpty(Style))
                 .AsString();

        #endregion
    }
}
