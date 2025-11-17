using System;
using System.Threading.Tasks;

namespace GreenSwampWebView;

/// <summary>
/// Platform abstraction interface for WebView implementations.
/// </summary>
public interface IWebViewPlatform : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the platform can go back in navigation history.
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// Gets a value indicating whether the platform can go forward in navigation history.
    /// </summary>
    bool CanGoForward { get; }

    /// <summary>
    /// Occurs when navigation is starting.
    /// </summary>
    event EventHandler<WebViewNavigationEventArgs>? NavigationStarting;

    /// <summary>
    /// Occurs when navigation has completed.
    /// </summary>
    event EventHandler<WebViewNavigationEventArgs>? NavigationCompleted;

    /// <summary>
    /// Occurs when navigation has failed.
    /// </summary>
    event EventHandler<WebViewNavigationEventArgs>? NavigationFailed;

    /// <summary>
    /// Initializes the platform-specific WebView.
    /// </summary>
    /// <param name="parentHandle">Native handle of the parent window.</param>
    /// <returns>A task representing the asynchronous initialization.</returns>
    Task InitializeAsync(IntPtr parentHandle);

    /// <summary>
    /// Navigates to the specified URL.
    /// </summary>
    /// <param name="url">The URL to navigate to.</param>
    void Navigate(string url);

    /// <summary>
    /// Navigates to the specified HTML string.
    /// </summary>
    /// <param name="html">The HTML content to display.</param>
    void NavigateToString(string html);

    /// <summary>
    /// Navigates back in the navigation history.
    /// </summary>
    void GoBack();

    /// <summary>
    /// Navigates forward in the navigation history.
    /// </summary>
    void GoForward();

    /// <summary>
    /// Reloads the current page.
    /// </summary>
    void Reload();

    /// <summary>
    /// Stops loading the current page.
    /// </summary>
    void Stop();

    /// <summary>
    /// Executes the specified JavaScript code.
    /// </summary>
    /// <param name="script">The JavaScript code to execute.</param>
    /// <returns>A task representing the asynchronous operation with the result.</returns>
    Task<string> ExecuteScriptAsync(string script);

    /// <summary>
    /// Updates the bounds of the WebView.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    void UpdateBounds(int x, int y, int width, int height);

    /// <summary>
    /// Sets the visibility of the WebView.
    /// </summary>
    /// <param name="visible">True to make the WebView visible; otherwise, false.</param>
    void SetVisible(bool visible);
}
