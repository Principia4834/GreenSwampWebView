using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GreenSwampWebView.Platforms.Linux;

/// <summary>
/// Linux implementation of WebView using WebKitGTK.
/// </summary>
public class LinuxWebView : IWebViewPlatform
{
    private IntPtr _webView = IntPtr.Zero;
    private IntPtr _parentWindow = IntPtr.Zero;
    private bool _isInitialized;

    /// <inheritdoc/>
    public bool CanGoBack
    {
        get
        {
#if LINUX
            if (_webView != IntPtr.Zero)
            {
                return WebKitGtkNative.CanGoBack(_webView);
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
#if LINUX
            if (_webView != IntPtr.Zero)
            {
                return WebKitGtkNative.CanGoForward(_webView);
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
#if LINUX
        try
        {
            _parentWindow = parentHandle;

            // Initialize GTK if not already initialized
            WebKitGtkNative.InitGtk();

            // Create WebKitGTK WebView
            _webView = WebKitGtkNative.CreateWebView();

            if (_webView != IntPtr.Zero)
            {
                // Add to parent
                WebKitGtkNative.AddToParent(_webView, parentHandle);
                _isInitialized = true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"WebKitGTK initialization failed: {ex.Message}");
        }
#endif
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Navigate(string url)
    {
#if LINUX
        if (_isInitialized && _webView != IntPtr.Zero)
        {
            WebKitGtkNative.LoadUri(_webView, url);
            NavigationStarting?.Invoke(this, new WebViewNavigationEventArgs(url, true));
        }
#endif
    }

    /// <inheritdoc/>
    public void NavigateToString(string html)
    {
#if LINUX
        if (_isInitialized && _webView != IntPtr.Zero)
        {
            WebKitGtkNative.LoadHtml(_webView, html);
            NavigationStarting?.Invoke(this, new WebViewNavigationEventArgs(null, true));
        }
#endif
    }

    /// <inheritdoc/>
    public void GoBack()
    {
#if LINUX
        if (_isInitialized && _webView != IntPtr.Zero && CanGoBack)
        {
            WebKitGtkNative.GoBack(_webView);
        }
#endif
    }

    /// <inheritdoc/>
    public void GoForward()
    {
#if LINUX
        if (_isInitialized && _webView != IntPtr.Zero && CanGoForward)
        {
            WebKitGtkNative.GoForward(_webView);
        }
#endif
    }

    /// <inheritdoc/>
    public void Reload()
    {
#if LINUX
        if (_isInitialized && _webView != IntPtr.Zero)
        {
            WebKitGtkNative.Reload(_webView);
        }
#endif
    }

    /// <inheritdoc/>
    public void Stop()
    {
#if LINUX
        if (_isInitialized && _webView != IntPtr.Zero)
        {
            WebKitGtkNative.StopLoading(_webView);
        }
#endif
    }

    /// <inheritdoc/>
    public async Task<string> ExecuteScriptAsync(string script)
    {
#if LINUX
        if (_isInitialized && _webView != IntPtr.Zero)
        {
            try
            {
                WebKitGtkNative.RunJavaScript(_webView, script);
                // Note: WebKitGTK's javascript execution is asynchronous
                // A full implementation would need callbacks
                return await Task.FromResult(string.Empty);
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
#if LINUX
        if (_webView != IntPtr.Zero)
        {
            WebKitGtkNative.SetSizeRequest(_webView, width, height);
        }
#endif
    }

    /// <inheritdoc/>
    public void SetVisible(bool visible)
    {
#if LINUX
        if (_webView != IntPtr.Zero)
        {
            WebKitGtkNative.SetVisible(_webView, visible);
        }
#endif
    }

    /// <inheritdoc/>
    public void Dispose()
    {
#if LINUX
        if (_webView != IntPtr.Zero)
        {
            WebKitGtkNative.Destroy(_webView);
            _webView = IntPtr.Zero;
        }

        _isInitialized = false;
#endif
    }
}
