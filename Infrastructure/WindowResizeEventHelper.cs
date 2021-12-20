using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public class WindowResizeEventHelper
    {
        private readonly Func<EventArgs, Task> _callback;

        public WindowResizeEventHelper(Func<EventArgs, Task> callback)
        {
            _callback = callback;
        }

        [JSInvokable]
        public Task OnWindowResizeEvent(EventArgs args) => _callback(args);
    }

    public class WindowResizeEventInterop : IDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<WindowResizeEventHelper> Reference;

        public WindowResizeEventInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask<string> SetupWindowResizeEventCallback(Func<EventArgs, Task> callback)
        {
            Reference = DotNetObjectReference.Create(new WindowResizeEventHelper(callback));
            return _jsRuntime.InvokeAsync<string>("addWindowResizeEventListener", Reference);
        }

        public void Dispose()
        {
            _jsRuntime.InvokeAsync<string>("removeWindowResizeEventListener", Reference);
            //Reference?.Dispose();
        }
    }
}
