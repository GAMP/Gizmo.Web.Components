using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Gizmo.Web.Components
{
    /// <summary>
    /// Renders the tooltip for every chart in the application. Place one of these in the layout,
    /// alongside the other hosts.
    /// </summary>
    /// <remarks>
    /// Charts do not render their own tooltip: a chart library paints one into its canvas bitmap,
    /// where a small canvas clips it. Instead each chart reports what its tooltip would say, and
    /// this single host draws it as ordinary DOM positioned against the viewport, so it is never
    /// confined by whatever container the chart sits in.
    /// <para>
    /// Charts opt in through their own script, by installing the shared tooltip on the charting
    /// library's defaults; nothing has to be added to an individual chart.
    /// </para>
    /// </remarks>
    public partial class ChartTooltipHost : CustomDOMComponentBase
    {
        #region FIELDS

        private DotNetObjectReference<ChartTooltipHost> _reference;

        private bool _isVisible;
        private double _x;
        private double _y;
        private IReadOnlyList<string> _titleLines = Array.Empty<string>();
        private IReadOnlyList<string> _bodyLines = Array.Empty<string>();

        #endregion

        #region FUNCTIONS

        /// <summary>
        /// Called from a chart whenever its tooltip changes.
        /// </summary>
        /// <remarks>
        /// The lines arrive already resolved by the chart, so any formatting it was configured
        /// with -- including amounts formatted in C# -- is preserved as-is. Coordinates are in
        /// viewport space.
        /// </remarks>
        [JSInvokable]
        public Task OnChartTooltipChanged(bool visible, double x, double y, string[] titleLines, string[] bodyLines)
        {
            _isVisible = visible;
            _x = x;
            _y = y;
            _titleLines = titleLines ?? Array.Empty<string>();
            _bodyLines = bodyLines ?? Array.Empty<string>();

            return InvokeAsync(StateHasChanged);
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _reference = CreateDotNetObjectReference(this);
                await InvokeVoidAsync("registerChartTooltipHost", _reference);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public override void Dispose()
        {
            if (_reference != null)
            {
                // Fire and forget: the host is going away, and on teardown the interop call may no
                // longer be possible at all.
                try
                {
                    _ = InvokeVoidAsync("unregisterChartTooltipHost");
                }
                catch (Exception)
                {
                }

                _reference.Dispose();
                _reference = null;
            }

            base.Dispose();
        }

        #endregion
    }
}
