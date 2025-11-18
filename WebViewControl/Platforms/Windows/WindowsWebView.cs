using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Web.WebView2.Core;
using System.Drawing;

namespace GreenSwampWebView.Platforms.Windows;

/// <summary>
/// Windows implementation of WebView using WebView2 managed APIs.
/// </summary>
public class WindowsWebView : IWebViewPlatform
{
#if WINDOWS
    private CoreWebView2Controller? _controller;
    private CoreWebView2? _coreWebView;
    private IntPtr _parentHandle;
    private IntPtr _childWindow;
    private bool _isInitialized;
    private TaskCompletionSource<bool>? _initializationTcs;
#endif

    /// <inheritdoc/>
    public bool CanGoBack
    {
        get
        {
#if WINDOWS
            try
            {
                return _coreWebView?.CanGoBack == true;
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
                return _coreWebView?.CanGoForward == true;
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
            // Create the child window for hosting the WebView2 control if needed.
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

            // Create managed WebView2 environment and controller (async).
            // IMPORTANT: InitializeAsync should be called from the UI (STA) thread so the continuation
            // runs on the UI context and the CoreWebView2Controller/CoreWebView2 are accessed on the UI thread.
            var env = await CoreWebView2Environment.CreateAsync(null, null, null);
            var controller = await env.CreateCoreWebView2ControllerAsync(_childWindow);

            _controller = controller;
            _coreWebView = _controller.CoreWebView2;

            // Hook up the managed events
            _coreWebView.NavigationStarting += CoreWebView_NavigationStarting;
            _coreWebView.NavigationCompleted += CoreWebView_NavigationCompleted;

            _initializationTcs?.TrySetResult(true);
            _isInitialized = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"WebView2 initialization failed: {ex.GetType().FullName} HResult=0x{ex.HResult:X8} Message={ex.Message}");
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
                return await _coreWebView.ExecuteScriptAsync(script) ?? string.Empty;
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
                _controller.Bounds = new Rectangle(0, 0, width, height);
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
                _controller.IsVisible = visible;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SetVisible failed: {ex.Message}");
            }
        }
#endif
    }

#if WINDOWS
    private void CoreWebView_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        try
        {
            var uri = e.Uri;
            NavigationStarting?.Invoke(this, new WebViewNavigationEventArgs(uri, true));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"NavigationStarting handler failed: {ex.Message}");
        }
    }

    private void CoreWebView_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        try
        {
            var uri = _coreWebView?.Source ?? string.Empty;
            if (e.IsSuccess)
            {
                NavigationCompleted?.Invoke(this, new WebViewNavigationEventArgs(uri, true));
            }
            else
            {
                var errorMessage = $"Navigation failed with status: {e.WebErrorStatus}";
                NavigationFailed?.Invoke(this, new WebViewNavigationEventArgs(uri, false, errorMessage));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"NavigationCompleted handler failed: {ex.Message}");
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
                _coreWebView.NavigationStarting -= CoreWebView_NavigationStarting;
                _coreWebView.NavigationCompleted -= CoreWebView_NavigationCompleted;
                _coreWebView = null;
            }

            if (_controller != null)
            {
                try { _controller.Close(); } catch { }
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
