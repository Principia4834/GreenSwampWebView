using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GreenSwampWebView.Platforms.Windows;

/// <summary>
/// Windows implementation of WebView using WebView2.
/// </summary>
public class WindowsWebView : IWebViewPlatform
{
#if WINDOWS
    private Microsoft.Web.WebView2.WinForms.WebView2? _webView2;
    private IntPtr _parentHandle;
    private bool _isInitialized;
#endif

    /// <inheritdoc/>
    public bool CanGoBack
    {
        get
        {
#if WINDOWS
            return _webView2?.CanGoBack ?? false;
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
            return _webView2?.CanGoForward ?? false;
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

        try
        {
            _webView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            
            // Set parent
            SetParent(_webView2.Handle, parentHandle);

            // Initialize WebView2 environment and control
            await _webView2.EnsureCoreWebView2Async(null);

            // Subscribe to events
            if (_webView2.CoreWebView2 != null)
            {
                _webView2.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
                _webView2.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            }

            _isInitialized = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"WebView2 initialization failed: {ex.Message}");
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
        if (_isInitialized && _webView2?.CoreWebView2 != null)
        {
            _webView2.CoreWebView2.Navigate(url);
        }
#endif
    }

    /// <inheritdoc/>
    public void NavigateToString(string html)
    {
#if WINDOWS
        if (_isInitialized && _webView2?.CoreWebView2 != null)
        {
            _webView2.CoreWebView2.NavigateToString(html);
        }
#endif
    }

    /// <inheritdoc/>
    public void GoBack()
    {
#if WINDOWS
        if (_isInitialized && _webView2?.CoreWebView2 != null && CanGoBack)
        {
            _webView2.CoreWebView2.GoBack();
        }
#endif
    }

    /// <inheritdoc/>
    public void GoForward()
    {
#if WINDOWS
        if (_isInitialized && _webView2?.CoreWebView2 != null && CanGoForward)
        {
            _webView2.CoreWebView2.GoForward();
        }
#endif
    }

    /// <inheritdoc/>
    public void Reload()
    {
#if WINDOWS
        if (_isInitialized && _webView2?.CoreWebView2 != null)
        {
            _webView2.CoreWebView2.Reload();
        }
#endif
    }

    /// <inheritdoc/>
    public void Stop()
    {
#if WINDOWS
        if (_isInitialized && _webView2?.CoreWebView2 != null)
        {
            _webView2.CoreWebView2.Stop();
        }
#endif
    }

    /// <inheritdoc/>
    public async Task<string> ExecuteScriptAsync(string script)
    {
#if WINDOWS
        if (_isInitialized && _webView2?.CoreWebView2 != null)
        {
            try
            {
                return await _webView2.CoreWebView2.ExecuteScriptAsync(script);
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
        if (_webView2 != null)
        {
            SetWindowPos(_webView2.Handle, IntPtr.Zero, x, y, width, height, 0x0004); // SWP_NOZORDER
        }
#endif
    }

    /// <inheritdoc/>
    public void SetVisible(bool visible)
    {
#if WINDOWS
        if (_webView2 != null)
        {
            ShowWindow(_webView2.Handle, visible ? 5 : 0); // SW_SHOW : SW_HIDE
        }
#endif
    }

#if WINDOWS
    private void CoreWebView2_NavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
    {
        NavigationStarting?.Invoke(this, new WebViewNavigationEventArgs(e.Uri, true));
    }

    private void CoreWebView2_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
    {
        if (e.IsSuccess)
        {
            NavigationCompleted?.Invoke(this, new WebViewNavigationEventArgs(_webView2?.Source?.ToString(), true));
        }
        else
        {
            var errorMessage = $"Navigation failed with status: {e.WebErrorStatus}";
            NavigationFailed?.Invoke(this, new WebViewNavigationEventArgs(_webView2?.Source?.ToString(), false, errorMessage));
        }
    }

    [DllImport("user32.dll")]
    private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
#endif

    /// <inheritdoc/>
    public void Dispose()
    {
#if WINDOWS
        if (_webView2 != null)
        {
            if (_webView2.CoreWebView2 != null)
            {
                _webView2.CoreWebView2.NavigationStarting -= CoreWebView2_NavigationStarting;
                _webView2.CoreWebView2.NavigationCompleted -= CoreWebView2_NavigationCompleted;
            }

            _webView2.Dispose();
            _webView2 = null;
        }

        _isInitialized = false;
#endif
    }
}
