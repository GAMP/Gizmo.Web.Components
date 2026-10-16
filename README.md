# Gizmo.Web.Components

## Gulp installation steps:

* For scss/js compilation we are using gulp. The following packages are necessary:
  * gulp gulp-autoprefixer
  * gulp-clean-css gulp-concat
  * gulp-connect 
  * gulp-group-css-media-queries
  * gulp-less
  * gulp-rename
  * gulp-sass
  * gulp-sourcemaps
  * gulp-minify
  * postcss
  * node-sass

  and they are installed as dev-dependencies with Node.js 18.12.1.

* To install gulp-cli globally open the command line
  and run the following command:

  ```console
  npm install --global gulp-cli
  ```

* Under both Gizmo.Client.UI and Gizmo.Web.Components directories open command line
  and run the following command to install required packages:

  ```console 
  npm install
  ```

* After installation you can run "gulp" command to start watching for file changes in scss/js directories.

* Visual Studio 2022 - Task Runner Explorer
  If Task Runner Explorer cannot load Gulpfile.js with error: ".. could not find a binding for your current environment: ...",
  you can try to move the (PATH) above the (VSInstalledExternalTools) under Tools->Options...->Projects and Solutions->Web Package Management
  and restart Visual Studio.

## Consuming the package (chart tooltip)

The package ships the chart tooltip's browser assets, served by the host at
`_content/Gizmo.Web.Components/...`:

| Asset | URL |
| --- | --- |
| Tooltip interop module | `_content/Gizmo.Web.Components/js/chart-tooltip.js` |
| Tooltip geometry | `_content/Gizmo.Web.Components/css/chart-tooltip.css` |

`ChartTooltipHost` renders the tooltip for every chart in the application, so place one
instance in the layout, alongside the other hosts:

```razor
<ChartTooltipHost />
```

### Required setup

1. Link the stylesheet. It carries geometry only — colours, typography and the horizontal
   anchor belong to the application, which styles `giz-chart-tooltip` against the
   `--giz-chart-tooltip-x` / `--giz-chart-tooltip-y` custom properties the component publishes.

   ```html
   <link rel="stylesheet" href="_content/Gizmo.Web.Components/css/chart-tooltip.css" />
   ```

   In a Razor component, `@Assets["_content/Gizmo.Web.Components/css/chart-tooltip.css"]`
   resolves the same URL with the host's asset fingerprinting.

2. Load the interop module with `type="module"` and make sure it has been evaluated before the
   first interactive render, in the host page (`App.razor` for a Blazor Web App,
   `wwwroot/index.html` for Blazor WebAssembly). Module scripts are deferred, so tagging the
   module before the framework script does not order the two: the framework script runs at its
   position during parsing, ahead of the deferred module. Start Blazor from the module after
   awaiting the interop import:

   ```html
   <script src="_framework/blazor.web.js" autostart="false"></script>
   <script type="module">
       await import("/_content/Gizmo.Web.Components/js/chart-tooltip.js");
       await Blazor.start();
   </script>
   ```

   The module assigns `window.registerChartTooltipHost` and
   `window.unregisterChartTooltipHost` while it is being evaluated, and `ChartTooltipHost`
   calls `registerChartTooltipHost` on its first interactive render (`OnAfterRenderAsync`), so
   the import has to complete before `Blazor.start()` resolves. Loading the module later can fail
   with `Could not find 'registerChartTooltipHost'`. Static (non-interactive) rendering never
   calls the globals, so an SSR-only page needs no script. The same `autostart="false"` and
   `Blazor.start()` pattern applies to `blazor.server.js` and `blazor.webassembly.js`.

3. Make charts report to the host. `installChartTooltip(Chart)` is exported from the same
   module and switches the chart library's own canvas tooltip off, so every chart created
   afterwards reports to the host instead. Import it by URL, relative to the app's base path:

   ```js
   const { installChartTooltip } = await import("/_content/Gizmo.Web.Components/js/chart-tooltip.js");

   installChartTooltip(Chart); // once, after importing the chart library, before creating charts
   ```

   The chart library is the caller's dependency and is not referenced by this package. A bundle
   that builds its own copy of the chart library calls this once per bundle, because the
   defaults it changes are per copy. `hideChartTooltip()` and `noChartTooltip()` are exported
   alongside it.

### Serving

`_content/Gizmo.Web.Components/...` is served by `Microsoft.NET.Sdk.Web` hosts through their
static file middleware (`UseStaticFiles()` in .NET 8 and earlier, `MapStaticAssets()` in
.NET 9+). A host that does not serve static files has to expose the two files itself.

### Source and bundle consumers

Projects that build this repository's sources directly are unaffected: the interop module
stays at `src/js/chart-tooltip.js` for imports (for example from a webpack entry) and the
geometry rules stay authoritative in `src/scss/_chart-tooltip.scss`. `wwwroot/css/chart-tooltip.css`
is only the packaged copy of those rules, so regenerate it when the SCSS changes.

