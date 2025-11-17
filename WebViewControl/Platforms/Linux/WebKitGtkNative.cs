using System;
using System.Runtime.InteropServices;

namespace GreenSwampWebView.Platforms.Linux;

/// <summary>
/// Native P/Invoke declarations for WebKitGTK on Linux.
/// </summary>
internal static class WebKitGtkNative
{
#if LINUX
    private const string GtkLib = "libgtk-3.so.0";
    private const string WebKitLib = "libwebkit2gtk-4.0.so.37";

    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool gtk_init_check(int argc, string[] argv);

    [DllImport(WebKitLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr webkit_web_view_new();

    [DllImport(WebKitLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void webkit_web_view_load_uri(IntPtr webView, string uri);

    [DllImport(WebKitLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void webkit_web_view_load_html(IntPtr webView, string content, string? baseUri);

    [DllImport(WebKitLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void webkit_web_view_reload(IntPtr webView);

    [DllImport(WebKitLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void webkit_web_view_stop_loading(IntPtr webView);

    [DllImport(WebKitLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void webkit_web_view_go_back(IntPtr webView);

    [DllImport(WebKitLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void webkit_web_view_go_forward(IntPtr webView);

    [DllImport(WebKitLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool webkit_web_view_can_go_back(IntPtr webView);

    [DllImport(WebKitLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool webkit_web_view_can_go_forward(IntPtr webView);

    [DllImport(WebKitLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void webkit_web_view_run_javascript(IntPtr webView, string script, IntPtr cancellable, IntPtr callback, IntPtr userData);

    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void gtk_container_add(IntPtr container, IntPtr widget);

    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void gtk_widget_set_size_request(IntPtr widget, int width, int height);

    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void gtk_widget_show(IntPtr widget);

    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void gtk_widget_hide(IntPtr widget);

    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    private static extern void gtk_widget_destroy(IntPtr widget);

    public static void InitGtk()
    {
        gtk_init_check(0, Array.Empty<string>());
    }

    public static IntPtr CreateWebView()
    {
        return webkit_web_view_new();
    }

    public static void AddToParent(IntPtr webView, IntPtr parent)
    {
        gtk_container_add(parent, webView);
        gtk_widget_show(webView);
    }

    public static void LoadUri(IntPtr webView, string uri)
    {
        webkit_web_view_load_uri(webView, uri);
    }

    public static void LoadHtml(IntPtr webView, string html)
    {
        webkit_web_view_load_html(webView, html, null);
    }

    public static void Reload(IntPtr webView)
    {
        webkit_web_view_reload(webView);
    }

    public static void StopLoading(IntPtr webView)
    {
        webkit_web_view_stop_loading(webView);
    }

    public static void GoBack(IntPtr webView)
    {
        webkit_web_view_go_back(webView);
    }

    public static void GoForward(IntPtr webView)
    {
        webkit_web_view_go_forward(webView);
    }

    public static bool CanGoBack(IntPtr webView)
    {
        return webkit_web_view_can_go_back(webView);
    }

    public static bool CanGoForward(IntPtr webView)
    {
        return webkit_web_view_can_go_forward(webView);
    }

    public static void RunJavaScript(IntPtr webView, string script)
    {
        webkit_web_view_run_javascript(webView, script, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
    }

    public static void SetSizeRequest(IntPtr webView, int width, int height)
    {
        gtk_widget_set_size_request(webView, width, height);
    }

    public static void SetVisible(IntPtr webView, bool visible)
    {
        if (visible)
        {
            gtk_widget_show(webView);
        }
        else
        {
            gtk_widget_hide(webView);
        }
    }

    public static void Destroy(IntPtr webView)
    {
        gtk_widget_destroy(webView);
    }
#else
    public static void InitGtk() { }
    public static IntPtr CreateWebView() => IntPtr.Zero;
    public static void AddToParent(IntPtr webView, IntPtr parent) { }
    public static void LoadUri(IntPtr webView, string uri) { }
    public static void LoadHtml(IntPtr webView, string html) { }
    public static void Reload(IntPtr webView) { }
    public static void StopLoading(IntPtr webView) { }
    public static void GoBack(IntPtr webView) { }
    public static void GoForward(IntPtr webView) { }
    public static bool CanGoBack(IntPtr webView) => false;
    public static bool CanGoForward(IntPtr webView) => false;
    public static void RunJavaScript(IntPtr webView, string script) { }
    public static void SetSizeRequest(IntPtr webView, int width, int height) { }
    public static void SetVisible(IntPtr webView, bool visible) { }
    public static void Destroy(IntPtr webView) { }
#endif
}
