/** @format */

// A chart library paints its own tooltip into the canvas bitmap, so on a small canvas the tooltip
// is clipped at the edge with no way to style around it. This module makes every chart report its
// tooltip to a single component instead, which renders it as ordinary DOM.
//
// The host registers itself on `window` rather than being captured when the defaults are
// installed: bundles are built separately and each carries its own copy of the chart library, so
// the host reference has to be reachable from all of them.

window.registerChartTooltipHost = function registerChartTooltipHost(dotNetRef) {
    window.gizChartTooltipHost = dotNetRef;
};

window.unregisterChartTooltipHost = function unregisterChartTooltipHost() {
    window.gizChartTooltipHost = null;
};

// Only one tooltip is ever on screen, so a single record of what was last reported is enough.
var lastKey = null;

function report(visible, x, y, titleLines, bodyLines) {
    var host = window.gizChartTooltipHost;
    if (!host) return;

    // The hook below fires on every mouse move. Without this guard each move would cost an
    // interop call, so only an actual change in what is shown gets through.
    var key = visible ? titleLines.join("|") + "#" + bodyLines.join("|") + "#" + x + ":" + y : "hidden";
    if (key === lastKey) return;

    lastKey = key;
    host.invokeMethodAsync("OnChartTooltipChanged", visible, x, y, titleLines, bodyLines);
}

function chartTooltipExternal(context) {
    try {
        var model = context.tooltip;

        // Visibility is taken from the active elements, not from the tooltip's opacity: opacity is
        // an animated property, and this hook runs in the same turn that schedules the animation,
        // so it still reads 0 when a tooltip is appearing. Worse, a doughnut's caret sits at the
        // centre of a segment and does not move while the pointer stays inside it, so the hook is
        // not called a second time to correct the mistake.
        var active = typeof model.getActiveElements === "function" ? model.getActiveElements() : [];

        if (!active.length) {
            report(false, 0, 0, [], []);
            return;
        }

        // The lines are the ones the chart has already resolved, not raw values, so every existing
        // label callback keeps working -- including the ones that format currency in C#, which
        // JavaScript cannot reproduce.
        var titleLines = model.title ? model.title.slice() : [];
        var bodyLines = [];

        if (model.body) {
            model.body.forEach(function (item) {
                if (item && item.lines) {
                    item.lines.forEach(function (line) {
                        bodyLines.push(String(line));
                    });
                }
            });
        }

        // The caret is animated too, so it is read from the active elements instead. Averaging
        // them mirrors what the chart's own default positioner does, which matters for the charts
        // that put several points in one tooltip.
        var sumX = 0;
        var sumY = 0;
        var counted = 0;

        active.forEach(function (item) {
            var element = item.element;
            if (element && typeof element.tooltipPosition === "function") {
                // `true` asks for the final position rather than the one being animated towards.
                var point = element.tooltipPosition(true);
                sumX += point.x;
                sumY += point.y;
                counted++;
            }
        });

        var caretX = counted ? sumX / counted : model.caretX;
        var caretY = counted ? sumY / counted : model.caretY;

        // Canvas coordinates mean nothing to an element positioned against the viewport, so they
        // are translated here. This is what frees the tooltip from the chart's container.
        var rect = context.chart.canvas.getBoundingClientRect();

        report(true, Math.round(rect.left + caretX), Math.round(rect.top + caretY), titleLines, bodyLines);
    } catch (e) {
        console.error(e.message);
    }
}

/**
 * Hides the tooltip. Call this when a chart goes away while its tooltip is up: a destroyed chart
 * reports nothing further, so the tooltip would otherwise stay on screen.
 */
export function hideChartTooltip() {
    report(false, 0, 0, [], []);
}

/**
 * A no-op stand-in for charts that should not show a tooltip at all.
 */
export function noChartTooltip() {
    return function () { };
}

/**
 * Makes the shared tooltip the default for every chart created from this bundle. Call once, after
 * importing the chart library and before any chart is built.
 *
 * @param {object} Chart The chart library's entry object.
 */
export function installChartTooltip(Chart) {
    Chart.defaults.plugins.tooltip.enabled = false;
    Chart.defaults.plugins.tooltip.external = chartTooltipExternal;
}
