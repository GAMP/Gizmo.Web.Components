using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public class WindowClickEventHelper
    {
        private readonly Func<MouseEventArgs, Task> _callback;

        public WindowClickEventHelper(Func<MouseEventArgs, Task> callback)
        {
            _callback = callback;
        }

        [JSInvokable]
        public Task OnWindowClickEvent(MouseEventArgs args) => _callback(args);
    }

    public class WindowClickEventInterop : IDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<WindowClickEventHelper> Reference;

        public WindowClickEventInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask<string> SetupWindowClickEventCallback(Func<MouseEventArgs, Task> callback)
        {
            Reference = DotNetObjectReference.Create(new WindowClickEventHelper(callback));
            return _jsRuntime.InvokeAsync<string>("addWindowClickEventListener", Reference);
        }

        public void Dispose()
        {
            _jsRuntime.InvokeAsync<string>("removeWindowClickEventListener", Reference);
            //Reference?.Dispose();
        }
    }
}
