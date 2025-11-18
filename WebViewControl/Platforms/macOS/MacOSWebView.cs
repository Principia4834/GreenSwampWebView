using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GreenSwampWebView.Platforms.macOS;

/// <summary>
/// macOS implementation of WebView using WKWebView.
/// </summary>
public class MacOSWebView : IWebViewPlatform
{
    private IntPtr _wkWebView = IntPtr.Zero;
    private IntPtr _parentView = IntPtr.Zero;
    private bool _isInitialized;

    /// <inheritdoc/>
    public bool CanGoBack
    {
        get
        {
#if MACOS
            if (_wkWebView != IntPtr.Zero)
            {
                return WKWebViewNative.CanGoBack(_wkWebView);
            }
#endif
            return false;
        }
    }

    /// <inheritdoc/>
    public bool CanGoForward
    {
        get
        {
#if MACOS
            if (_wkWebView != IntPtr.Zero)
            {
                return WKWebViewNative.CanGoForward(_wkWebView);
            }
#endif
            return false;
        }
    }

    /// <inheritdoc/>
    public event EventHandler<WebViewNavigationEventArgs>? NavigationStarting;

    /// <inheritdoc/>
    public event EventHandler<WebViewNavigationEventArgs>? NavigationCompleted;

    /// <inheritdoc/>
    public event EventHandler<WebViewNavigationEventArgs>? NavigationFailed;

    /// <inheritdoc/>
    public Task InitializeAsync(IntPtr parentHandle)
    {
#if MACOS
        try
        {
            _parentView = parentHandle;
            _wkWebView = WKWebViewNative.CreateWKWebView();

            if (_wkWebView != IntPtr.Zero)
            {
                WKWebViewNative.AddToParent(_wkWebView, parentHandle);
                _isInitialized = true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"WKWebView initialization failed: {ex.Message}");
        }
#endif
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Navigate(string url)
    {
#if MACOS
        if (_isInitialized && _wkWebView != IntPtr.Zero)
        {
            WKWebViewNative.LoadUrl(_wkWebView, url);
            NavigationStarting?.Invoke(this, new WebViewNavigationEventArgs(url, true));
        }
#endif
    }

    /// <inheritdoc/>
    public void NavigateToString(string html)
    {
#if MACOS
        if (_isInitialized && _wkWebView != IntPtr.Zero)
        {
            WKWebViewNative.LoadHtmlString(_wkWebView, html);
            NavigationStarting?.Invoke(this, new WebViewNavigationEventArgs(null, true));
        }
#endif
    }

    /// <inheritdoc/>
    public void GoBack()
    {
#if MACOS
        if (_isInitialized && _wkWebView != IntPtr.Zero && CanGoBack)
        {
            WKWebViewNative.GoBack(_wkWebView);
        }
#endif
    }

    /// <inheritdoc/>
    public void GoForward()
    {
#if MACOS
        if (_isInitialized && _wkWebView != IntPtr.Zero && CanGoForward)
        {
            WKWebViewNative.GoForward(_wkWebView);
        }
#endif
    }

    /// <inheritdoc/>
    public void Reload()
    {
#if MACOS
        if (_isInitialized && _wkWebView != IntPtr.Zero)
        {
            WKWebViewNative.Reload(_wkWebView);
        }
#endif
    }

    /// <inheritdoc/>
    public void Stop()
    {
#if MACOS
        if (_isInitialized && _wkWebView != IntPtr.Zero)
        {
            WKWebViewNative.StopLoading(_wkWebView);
        }
#endif
    }

    /// <inheritdoc/>
    public async Task<string> ExecuteScriptAsync(string script)
    {
#if MACOS
        if (_isInitialized && _wkWebView != IntPtr.Zero)
        {
            try
            {
                var result = WKWebViewNative.EvaluateJavaScript(_wkWebView, script);
                return await Task.FromResult(result ?? string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Script execution failed: {ex.Message}");
            }
        }
#endif
        return await Task.FromResult(string.Empty);
    }

    /// <inheritdoc/>
    public void UpdateBounds(int x, int y, int width, int height)
    {
#if MACOS
        if (_wkWebView != IntPtr.Zero)
        {
            WKWebViewNative.SetFrame(_wkWebView, x, y, width, height);
        }
#endif
    }

    /// <inheritdoc/>
    public void SetVisible(bool visible)
    {
#if MACOS
        if (_wkWebView != IntPtr.Zero)
        {
            WKWebViewNative.SetHidden(_wkWebView, !visible);
        }
#endif
    }

    /// <inheritdoc/>
    public void Dispose()
    {
#if MACOS
        if (_wkWebView != IntPtr.Zero)
        {
            WKWebViewNative.Release(_wkWebView);
            _wkWebView = IntPtr.Zero;
        }

        _isInitialized = false;
#endif
    }
}
