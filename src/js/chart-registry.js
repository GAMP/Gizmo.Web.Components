/** @format */

// Charts live in a shared `window.charts` registry keyed by their canvas id, so that .NET can
// replace or release one by id. A component that goes away without releasing its chart leaves the
// instance behind, still holding a detached canvas and its resize observer.
//
// This is a safety net, not a substitute for releasing a chart when its owner is disposed. It
// exists because not every chart in the product has an owner that does so.

/**
 * Destroys and forgets every chart whose canvas has left the document.
 *
 * @returns {number} How many were swept.
 */
export function pruneDetachedCharts() {
    var registry = window.charts;
    if (!registry) return 0;

    var removed = 0;

    Object.keys(registry).forEach(function (key) {
        var chart = registry[key];
        var canvas = chart && chart.canvas;

        // Still on the page, so it can still be shown.
        if (canvas && canvas.isConnected) return;

        try {
            if (chart && typeof chart.destroy === "function") {
                chart.destroy();
            }
        } catch (e) {
            console.error(e.message);
        }

        delete registry[key];
        removed++;
    });

    return removed;
}
