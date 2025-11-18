using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GreenSwampWebView.Platforms.Windows;

/// <summary>
/// Windows implementation of WebView using WebView2 COM interfaces.
/// </summary>
public class WindowsWebView : IWebViewPlatform
{
#if WINDOWS
    private WebView2Native.ICoreWebView2Controller? _controller;
    private WebView2Native.ICoreWebView2? _coreWebView;
    private IntPtr _parentHandle;
    private IntPtr _childWindow;
    private bool _isInitialized;
    private TaskCompletionSource<bool>? _initializationTcs;
    private long _navigationStartingToken;
    private long _navigationCompletedToken;
#endif

    /// <inheritdoc/>
    public bool CanGoBack
    {
        get
        {
#if WINDOWS
            try
            {
                return _coreWebView?.CanGoBack == 1;
            }
            catch
            {
                return false;
            }
#else
            return false;
#endif
        }
    }

    /// <inheritdoc/>
    public bool CanGoForward
    {
        get
        {
#if WINDOWS
            try
            {
                return _coreWebView?.CanGoForward == 1;
            }
            catch
            {
                return false;
            }
#else
            return false;
#endif
        }
    }

    /// <inheritdoc/>
    public event EventHandler<WebViewNavigationEventArgs>? NavigationStarting;

    /// <inheritdoc/>
    public event EventHandler<WebViewNavigationEventArgs>? NavigationCompleted;

    /// <inheritdoc/>
    public event EventHandler<WebViewNavigationEventArgs>? NavigationFailed;

    /// <inheritdoc/>
    public async Task InitializeAsync(IntPtr parentHandle)
    {
#if WINDOWS
        _parentHandle = parentHandle;
        _initializationTcs = new TaskCompletionSource<bool>();

        try
        {
            // Create a child window for WebView2
            _childWindow = WebView2Native.CreateWindowEx(
                0,
                "Static",
                "",
                WebView2Native.WS_CHILD | WebView2Native.WS_VISIBLE | WebView2Native.WS_CLIPCHILDREN | WebView2Native.WS_CLIPSIBLINGS,
                0, 0, 100, 100,
                parentHandle,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero);

            if (_childWindow == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to create child window for WebView2");
            }

            // Create WebView2 environment and controller
            var environmentHandler = new CreateEnvironmentHandler(this);
            WebView2Native.CreateCoreWebView2EnvironmentWithOptions(null, null, IntPtr.Zero, environmentHandler);

            // Wait for initialization to complete
            await _initializationTcs.Task;
            _isInitialized = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"WebView2 initialization failed: {ex.Message}");
            _initializationTcs?.TrySetException(ex);
            throw;
        }
#else
        await Task.CompletedTask;
#endif
    }

    /// <inheritdoc/>
    public void Navigate(string url)
    {
#if WINDOWS
        if (_isInitialized && _coreWebView != null)
        {
            try
            {
                _coreWebView.Navigate(url);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation failed: {ex.Message}");
            }
        }
#endif
    }

    /// <inheritdoc/>
    public void NavigateToString(string html)
    {
#if WINDOWS
        if (_isInitialized && _coreWebView != null)
        {
            try
            {
                _coreWebView.NavigateToString(html);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NavigateToString failed: {ex.Message}");
            }
        }
#endif
    }

    /// <inheritdoc/>
    public void GoBack()
    {
#if WINDOWS
        if (_isInitialized && _coreWebView != null && CanGoBack)
        {
            try
            {
                _coreWebView.GoBack();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GoBack failed: {ex.Message}");
            }
        }
#endif
    }

    /// <inheritdoc/>
    public void GoForward()
    {
#if WINDOWS
        if (_isInitialized && _coreWebView != null && CanGoForward)
        {
            try
            {
                _coreWebView.GoForward();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GoForward failed: {ex.Message}");
            }
        }
#endif
    }

    /// <inheritdoc/>
    public void Reload()
    {
#if WINDOWS
        if (_isInitialized && _coreWebView != null)
        {
            try
            {
                _coreWebView.Reload();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Reload failed: {ex.Message}");
            }
        }
#endif
    }

    /// <inheritdoc/>
    public void Stop()
    {
#if WINDOWS
        if (_isInitialized && _coreWebView != null)
        {
            try
            {
                _coreWebView.Stop();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Stop failed: {ex.Message}");
            }
        }
#endif
    }

    /// <inheritdoc/>
    public async Task<string> ExecuteScriptAsync(string script)
    {
#if WINDOWS
        if (_isInitialized && _coreWebView != null)
        {
            try
            {
                var tcs = new TaskCompletionSource<string>();
                var handler = new ExecuteScriptHandler(tcs);
                _coreWebView.ExecuteScript(script, handler);
                return await tcs.Task;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Script execution failed: {ex.Message}");
                return string.Empty;
            }
        }
#endif
        return await Task.FromResult(string.Empty);
    }

    /// <inheritdoc/>
    public void UpdateBounds(int x, int y, int width, int height)
    {
#if WINDOWS
        if (_childWindow != IntPtr.Zero)
        {
            WebView2Native.SetWindowPos(_childWindow, IntPtr.Zero, x, y, width, height, WebView2Native.SWP_NOZORDER);
        }

        if (_controller != null)
        {
            try
            {
                var bounds = new WebView2Native.tagRECT
                {
                    left = 0,
                    top = 0,
                    right = width,
                    bottom = height
                };
                _controller.put_Bounds(bounds);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateBounds failed: {ex.Message}");
            }
        }
#endif
    }

    /// <inheritdoc/>
    public void SetVisible(bool visible)
    {
#if WINDOWS
        if (_childWindow != IntPtr.Zero)
        {
            WebView2Native.ShowWindow(_childWindow, visible ? WebView2Native.SW_SHOW : WebView2Native.SW_HIDE);
        }

        if (_controller != null)
        {
            try
            {
                _controller.IsVisible = visible ? 1 : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SetVisible failed: {ex.Message}");
            }
        }
#endif
    }

#if WINDOWS
    // COM event handler implementations
    [ComVisible(true)]
    private class CreateEnvironmentHandler : WebView2Native.ICreateCoreWebView2EnvironmentCompletedHandler
    {
        private readonly WindowsWebView _parent;

        public CreateEnvironmentHandler(WindowsWebView parent)
        {
            _parent = parent;
        }

        public void Invoke(int errorCode, WebView2Native.ICoreWebView2Environment? createdEnvironment)
        {
            if (errorCode != 0 || createdEnvironment == null)
            {
                _parent._initializationTcs?.TrySetException(
                    new InvalidOperationException($"WebView2 environment creation failed with error code: {errorCode}"));
                return;
            }

            try
            {
                var controllerHandler = new CreateControllerHandler(_parent);
                createdEnvironment.CreateCoreWebView2Controller(_parent._childWindow, controllerHandler);
            }
            catch (Exception ex)
            {
                _parent._initializationTcs?.TrySetException(ex);
            }
        }
    }

    [ComVisible(true)]
    private class CreateControllerHandler : WebView2Native.ICreateCoreWebView2ControllerCompletedHandler
    {
        private readonly WindowsWebView _parent;

        public CreateControllerHandler(WindowsWebView parent)
        {
            _parent = parent;
        }

        public void Invoke(int errorCode, WebView2Native.ICoreWebView2Controller? createdController)
        {
            if (errorCode != 0 || createdController == null)
            {
                _parent._initializationTcs?.TrySetException(
                    new InvalidOperationException($"WebView2 controller creation failed with error code: {errorCode}"));
                return;
            }

            try
            {
                _parent._controller = createdController;
                _parent._coreWebView = createdController.CoreWebView2;

                // Set up event handlers
                var navStartingHandler = new NavigationStartingHandler(_parent);
                _parent._coreWebView.add_NavigationStarting(navStartingHandler, out _parent._navigationStartingToken);

                var navCompletedHandler = new NavigationCompletedHandler(_parent);
                _parent._coreWebView.add_NavigationCompleted(navCompletedHandler, out _parent._navigationCompletedToken);

                _parent._initializationTcs?.TrySetResult(true);
            }
            catch (Exception ex)
            {
                _parent._initializationTcs?.TrySetException(ex);
            }
        }
    }

    [ComVisible(true)]
    private class NavigationStartingHandler : WebView2Native.INavigationStartingEventHandler
    {
        private readonly WindowsWebView _parent;

        public NavigationStartingHandler(WindowsWebView parent)
        {
            _parent = parent;
        }

        public void Invoke(WebView2Native.ICoreWebView2 sender, WebView2Native.ICoreWebView2NavigationStartingEventArgs args)
        {
            try
            {
                var uri = args.Uri;
                _parent.NavigationStarting?.Invoke(_parent, new WebViewNavigationEventArgs(uri, true));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NavigationStarting event handler failed: {ex.Message}");
            }
        }
    }

    [ComVisible(true)]
    private class NavigationCompletedHandler : WebView2Native.INavigationCompletedEventHandler
    {
        private readonly WindowsWebView _parent;

        public NavigationCompletedHandler(WindowsWebView parent)
        {
            _parent = parent;
        }

        public void Invoke(WebView2Native.ICoreWebView2 sender, WebView2Native.ICoreWebView2NavigationCompletedEventArgs args)
        {
            try
            {
                var uri = sender.Source;
                if (args.IsSuccess == 1)
                {
                    _parent.NavigationCompleted?.Invoke(_parent, new WebViewNavigationEventArgs(uri, true));
                }
                else
                {
                    var errorMessage = $"Navigation failed with status: {args.WebErrorStatus}";
                    _parent.NavigationFailed?.Invoke(_parent, new WebViewNavigationEventArgs(uri, false, errorMessage));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NavigationCompleted event handler failed: {ex.Message}");
            }
        }
    }

    [ComVisible(true)]
    private class ExecuteScriptHandler : WebView2Native.IExecuteScriptCompletedHandler
    {
        private readonly TaskCompletionSource<string> _tcs;

        public ExecuteScriptHandler(TaskCompletionSource<string> tcs)
        {
            _tcs = tcs;
        }

        public void Invoke(int errorCode, string resultObjectAsJson)
        {
            if (errorCode != 0)
            {
                _tcs.TrySetException(new InvalidOperationException($"Script execution failed with error code: {errorCode}"));
            }
            else
            {
                _tcs.TrySetResult(resultObjectAsJson ?? string.Empty);
            }
        }
    }
#endif

    /// <inheritdoc/>
    public void Dispose()
    {
#if WINDOWS
        try
        {
            if (_coreWebView != null)
            {
                // Remove event handlers
                if (_navigationStartingToken != 0)
                {
                    _coreWebView.remove_NavigationStarting(_navigationStartingToken);
                }
                if (_navigationCompletedToken != 0)
                {
                    _coreWebView.remove_NavigationCompleted(_navigationCompletedToken);
                }

                // Release COM object
                Marshal.ReleaseComObject(_coreWebView);
                _coreWebView = null;
            }

            if (_controller != null)
            {
                _controller.Close();
                Marshal.ReleaseComObject(_controller);
                _controller = null;
            }

            if (_childWindow != IntPtr.Zero)
            {
                WebView2Native.DestroyWindow(_childWindow);
                _childWindow = IntPtr.Zero;
            }

            _isInitialized = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Dispose failed: {ex.Message}");
        }
#endif
    }
}
