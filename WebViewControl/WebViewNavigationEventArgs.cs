using System;

namespace GreenSwampWebView;

/// <summary>
/// Provides data for WebView navigation events.
/// </summary>
public class WebViewNavigationEventArgs : EventArgs
{
    /// <summary>
    /// Gets the URI of the navigation.
    /// </summary>
    public string? Uri { get; init; }

    /// <summary>
    /// Gets a value indicating whether the navigation was successful.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Gets the error message if the navigation failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets the HTTP status code for the navigation.
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebViewNavigationEventArgs"/> class.
    /// </summary>
    public WebViewNavigationEventArgs(string? uri, bool isSuccess = true, string? errorMessage = null, int statusCode = 200)
    {
        Uri = uri;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }
}
