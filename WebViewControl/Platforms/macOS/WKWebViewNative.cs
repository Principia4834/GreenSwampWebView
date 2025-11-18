using System;
using System.Runtime.InteropServices;

namespace GreenSwampWebView.Platforms.macOS;

/// <summary>
/// Native P/Invoke declarations for WKWebView on macOS.
/// </summary>
internal static class WKWebViewNative
{
#if MACOS
    private const string ObjectiveCLibrary = "/usr/lib/libobjc.dylib";
    private const string FoundationFramework = "/System/Library/Frameworks/Foundation.framework/Foundation";
    private const string WebKitFramework = "/System/Library/Frameworks/WebKit.framework/WebKit";

    [DllImport(ObjectiveCLibrary, EntryPoint = "objc_getClass")]
    private static extern IntPtr GetClass(string name);

    [DllImport(ObjectiveCLibrary, EntryPoint = "sel_registerName")]
    private static extern IntPtr GetSelector(string name);

    [DllImport(ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendMessage(IntPtr receiver, IntPtr selector);

    [DllImport(ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendMessage(IntPtr receiver, IntPtr selector, IntPtr arg1);

    [DllImport(ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendMessage(IntPtr receiver, IntPtr selector, double arg1, double arg2, double arg3, double arg4);

    [DllImport(ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
    private static extern bool SendMessageBool(IntPtr receiver, IntPtr selector);

    [DllImport(ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
    private static extern void SendMessageVoid(IntPtr receiver, IntPtr selector, bool arg1);

    public static IntPtr CreateWKWebView()
    {
        var wkWebViewClass = GetClass("WKWebView");
        var alloc = GetSelector("alloc");
        var init = GetSelector("init");

        var webView = SendMessage(wkWebViewClass, alloc);
        webView = SendMessage(webView, init);

        return webView;
    }

    public static void AddToParent(IntPtr webView, IntPtr parentView)
    {
        var addSubview = GetSelector("addSubview:");
        SendMessage(parentView, addSubview, webView);
    }

    public static void LoadUrl(IntPtr webView, string url)
    {
        var nsStringClass = GetClass("NSString");
        var stringWithUTF8String = GetSelector("stringWithUTF8String:");
        var urlString = SendMessage(nsStringClass, stringWithUTF8String, Marshal.StringToHGlobalAuto(url));

        var nsUrlClass = GetClass("NSURL");
        var urlWithString = GetSelector("URLWithString:");
        var nsUrl = SendMessage(nsUrlClass, urlWithString, urlString);

        var nsUrlRequestClass = GetClass("NSURLRequest");
        var requestWithURL = GetSelector("requestWithURL:");
        var request = SendMessage(nsUrlRequestClass, requestWithURL, nsUrl);

        var loadRequest = GetSelector("loadRequest:");
        SendMessage(webView, loadRequest, request);
    }

    public static void LoadHtmlString(IntPtr webView, string html)
    {
        var nsStringClass = GetClass("NSString");
        var stringWithUTF8String = GetSelector("stringWithUTF8String:");
        var htmlString = SendMessage(nsStringClass, stringWithUTF8String, Marshal.StringToHGlobalAuto(html));

        var loadHTMLString = GetSelector("loadHTMLString:baseURL:");
        SendMessage(webView, loadHTMLString, htmlString);
    }

    public static void GoBack(IntPtr webView)
    {
        var goBack = GetSelector("goBack");
        SendMessage(webView, goBack);
    }

    public static void GoForward(IntPtr webView)
    {
        var goForward = GetSelector("goForward");
        SendMessage(webView, goForward);
    }

    public static void Reload(IntPtr webView)
    {
        var reload = GetSelector("reload");
        SendMessage(webView, reload);
    }

    public static void StopLoading(IntPtr webView)
    {
        var stopLoading = GetSelector("stopLoading");
        SendMessage(webView, stopLoading);
    }

    public static bool CanGoBack(IntPtr webView)
    {
        var canGoBack = GetSelector("canGoBack");
        return SendMessageBool(webView, canGoBack);
    }

    public static bool CanGoForward(IntPtr webView)
    {
        var canGoForward = GetSelector("canGoForward");
        return SendMessageBool(webView, canGoForward);
    }

    public static string? EvaluateJavaScript(IntPtr webView, string script)
    {
        // Note: This is a simplified synchronous version
        // In production, WKWebView's evaluateJavaScript is asynchronous
        var nsStringClass = GetClass("NSString");
        var stringWithUTF8String = GetSelector("stringWithUTF8String:");
        var scriptString = SendMessage(nsStringClass, stringWithUTF8String, Marshal.StringToHGlobalAuto(script));

        var evaluateJavaScript = GetSelector("evaluateJavaScript:completionHandler:");
        SendMessage(webView, evaluateJavaScript, scriptString);

        return null; // Would need proper async handling
    }

    public static void SetFrame(IntPtr webView, int x, int y, int width, int height)
    {
        var setFrame = GetSelector("setFrame:");
        SendMessage(webView, setFrame, (double)x, (double)y, (double)width, (double)height);
    }

    public static void SetHidden(IntPtr webView, bool hidden)
    {
        var setHidden = GetSelector("setHidden:");
        SendMessageVoid(webView, setHidden, hidden);
    }

    public static void Release(IntPtr obj)
    {
        var release = GetSelector("release");
        SendMessage(obj, release);
    }
#else
    public static IntPtr CreateWKWebView() => IntPtr.Zero;
    public static void AddToParent(IntPtr webView, IntPtr parentView) { }
    public static void LoadUrl(IntPtr webView, string url) { }
    public static void LoadHtmlString(IntPtr webView, string html) { }
    public static void GoBack(IntPtr webView) { }
    public static void GoForward(IntPtr webView) { }
    public static void Reload(IntPtr webView) { }
    public static void StopLoading(IntPtr webView) { }
    public static bool CanGoBack(IntPtr webView) => false;
    public static bool CanGoForward(IntPtr webView) => false;
    public static string? EvaluateJavaScript(IntPtr webView, string script) => null;
    public static void SetFrame(IntPtr webView, int x, int y, int width, int height) { }
    public static void SetHidden(IntPtr webView, bool hidden) { }
    public static void Release(IntPtr obj) { }
#endif
}
