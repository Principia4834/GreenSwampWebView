using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using GreenSwampWebView.Platforms.Windows;
using GreenSwampWebView.Platforms.macOS;
using GreenSwampWebView.Platforms.Linux;

namespace GreenSwampWebView;

/// <summary>
/// A cross-platform WebView control for Avalonia UI.
/// </summary>
public class WebView : Control
{
    private IWebViewPlatform? _platformWebView;
    private string? _pendingUrl;
    private string? _pendingHtml;
    private bool _isInitialized;

    /// <summary>
    /// Defines the <see cref="Url"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> UrlProperty =
        AvaloniaProperty.Register<WebView, string?>(nameof(Url));

    /// <summary>
    /// Defines the <see cref="Html"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> HtmlProperty =
        AvaloniaProperty.Register<WebView, string?>(nameof(Html));

    /// <summary>
    /// Gets or sets the URL to navigate to.
    /// </summary>
    public string? Url
    {
        get => GetValue(UrlProperty);
        set => SetValue(UrlProperty, value);
    }

    /// <summary>
    /// Gets or sets the HTML content to display.
    /// </summary>
    public string? Html
    {
        get => GetValue(HtmlProperty);
        set => SetValue(HtmlProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether the WebView can navigate back.
    /// </summary>
    public bool CanGoBack => _platformWebView?.CanGoBack ?? false;

    /// <summary>
    /// Gets a value indicating whether the WebView can navigate forward.
    /// </summary>
    public bool CanGoForward => _platformWebView?.CanGoForward ?? false;

    /// <summary>
    /// Occurs when navigation is starting.
    /// </summary>
    public event EventHandler<WebViewNavigationEventArgs>? NavigationStarting;

    /// <summary>
    /// Occurs when navigation has completed.
    /// </summary>
    public event EventHandler<WebViewNavigationEventArgs>? NavigationCompleted;

    /// <summary>
    /// Occurs when navigation has failed.
    /// </summary>
    public event EventHandler<WebViewNavigationEventArgs>? NavigationFailed;

    static WebView()
    {
        AffectsRender<WebView>(UrlProperty, HtmlProperty);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebView"/> class.
    /// </summary>
    public WebView()
    {
        ClipToBounds = true;
    }

    /// <inheritdoc/>
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        InitializePlatformWebView();
    }

    /// <inheritdoc/>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        DisposePlatformWebView();
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == UrlProperty)
        {
            var url = change.GetNewValue<string?>();
            if (!string.IsNullOrEmpty(url))
            {
                Navigate(url);
            }
        }
        else if (change.Property == HtmlProperty)
        {
            var html = change.GetNewValue<string?>();
            if (!string.IsNullOrEmpty(html))
            {
                NavigateToString(html);
            }
        }
        else if (change.Property == BoundsProperty || change.Property == IsVisibleProperty)
        {
            UpdatePlatformWebView();
        }
    }

    /// <summary>
    /// Navigates to the specified URL.
    /// </summary>
    /// <param name="url">The URL to navigate to.</param>
    public void Navigate(string url)
    {
        if (_isInitialized && _platformWebView != null)
        {
            _platformWebView.Navigate(url);
            _pendingUrl = null;
        }
        else
        {
            _pendingUrl = url;
        }
    }

    /// <summary>
    /// Navigates to the specified HTML string.
    /// </summary>
    /// <param name="html">The HTML content to display.</param>
    public void NavigateToString(string html)
    {
        if (_isInitialized && _platformWebView != null)
        {
            _platformWebView.NavigateToString(html);
            _pendingHtml = null;
        }
        else
        {
            _pendingHtml = html;
        }
    }

    /// <summary>
    /// Navigates back in the navigation history.
    /// </summary>
    public void GoBack()
    {
        _platformWebView?.GoBack();
    }

    /// <summary>
    /// Navigates forward in the navigation history.
    /// </summary>
    public void GoForward()
    {
        _platformWebView?.GoForward();
    }

    /// <summary>
    /// Reloads the current page.
    /// </summary>
    public void Reload()
    {
        _platformWebView?.Reload();
    }

    /// <summary>
    /// Stops loading the current page.
    /// </summary>
    public void Stop()
    {
        _platformWebView?.Stop();
    }

    /// <summary>
    /// Executes the specified JavaScript code.
    /// </summary>
    /// <param name="script">The JavaScript code to execute.</param>
    /// <returns>A task representing the asynchronous operation with the result.</returns>
    public Task<string> ExecuteScriptAsync(string script)
    {
        if (_platformWebView == null)
        {
            return Task.FromResult(string.Empty);
        }

        return _platformWebView.ExecuteScriptAsync(script);
    }

    private async void InitializePlatformWebView()
    {
        if (_platformWebView != null || VisualRoot is not Window window)
        {
            return;
        }

        try
        {
            // Create platform-specific implementation
            _platformWebView = CreatePlatformWebView();

            if (_platformWebView == null)
            {
                return;
            }

            // Subscribe to events
            _platformWebView.NavigationStarting += OnPlatformNavigationStarting;
            _platformWebView.NavigationCompleted += OnPlatformNavigationCompleted;
            _platformWebView.NavigationFailed += OnPlatformNavigationFailed;

            // Get native window handle
            var windowHandle = GetNativeWindowHandle(window);

            // Initialize platform WebView
            await _platformWebView.InitializeAsync(windowHandle);
            _isInitialized = true;

            // Navigate to pending URL or HTML
            if (!string.IsNullOrEmpty(_pendingUrl))
            {
                Navigate(_pendingUrl);
            }
            else if (!string.IsNullOrEmpty(_pendingHtml))
            {
                NavigateToString(_pendingHtml);
            }

            UpdatePlatformWebView();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to initialize WebView: {ex.Message}");
        }
    }

    private IWebViewPlatform? CreatePlatformWebView()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new WindowsWebView();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return new MacOSWebView();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new LinuxWebView();
        }

        return null;
    }

    private IntPtr GetNativeWindowHandle(Window window)
    {
        if (window.TryGetPlatformHandle() is { } platformHandle)
        {
            return platformHandle.Handle;
        }

        return IntPtr.Zero;
    }

    private void UpdatePlatformWebView()
    {
        if (_platformWebView == null || !_isInitialized)
        {
            return;
        }

        var bounds = Bounds;
        _platformWebView.UpdateBounds(0, 0, (int)bounds.Width, (int)bounds.Height);
        _platformWebView.SetVisible(IsVisible);
    }

    private void DisposePlatformWebView()
    {
        if (_platformWebView != null)
        {
            _platformWebView.NavigationStarting -= OnPlatformNavigationStarting;
            _platformWebView.NavigationCompleted -= OnPlatformNavigationCompleted;
            _platformWebView.NavigationFailed -= OnPlatformNavigationFailed;
            _platformWebView.Dispose();
            _platformWebView = null;
            _isInitialized = false;
        }
    }

    private void OnPlatformNavigationStarting(object? sender, WebViewNavigationEventArgs e)
    {
        NavigationStarting?.Invoke(this, e);
    }

    private void OnPlatformNavigationCompleted(object? sender, WebViewNavigationEventArgs e)
    {
        NavigationCompleted?.Invoke(this, e);
    }

    private void OnPlatformNavigationFailed(object? sender, WebViewNavigationEventArgs e)
    {
        NavigationFailed?.Invoke(this, e);
    }

    /// <inheritdoc/>
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        
        // Draw a placeholder background when WebView is not initialized
        if (_platformWebView == null)
        {
            context.FillRectangle(Brushes.White, new Rect(Bounds.Size));
        }
    }
}
