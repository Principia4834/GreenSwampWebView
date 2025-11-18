using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GreenSwampWebView.Platforms.Windows;

/// <summary>
/// Native P/Invoke declarations and COM interfaces for WebView2 on Windows.
/// </summary>
internal static class WebView2Native
{
    // WebView2 Loader DLL
    // private const string WebView2LoaderDll = "WebView2Loader.dll";

    // COM Interface GUIDs
    public static readonly Guid IID_ICoreWebView2 = new Guid("76eceacb-0462-4d94-ac83-423a6793775e");
    public static readonly Guid IID_ICoreWebView2Controller = new Guid("4d00c0d1-9434-4eb6-8078-8697a560334f");
    public static readonly Guid IID_ICoreWebView2Environment = new Guid("b96d755e-0319-4e92-a296-23436f46a1fc");

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr CreateWindowEx(
        uint dwExStyle,
        string lpClassName,
        string lpWindowName,
        uint dwStyle,
        int x, int y, int nWidth, int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    [DllImport("user32.dll")]
    public static extern bool DestroyWindow(IntPtr hWnd);

    // COM Interfaces
    //[ComImport]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[Guid("4E8A3389-C9D8-4BD2-B6B5-124FEE6CC14D")]
    //public interface ICreateCoreWebView2EnvironmentCompletedHandler
    //{
    //    void Invoke([MarshalAs(UnmanagedType.Error)] int errorCode,
    //               [MarshalAs(UnmanagedType.Interface)] ICoreWebView2Environment? createdEnvironment);
    //}

    //[ComImport]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[Guid("b96d755e-0319-4e92-a296-23436f46a1fc")]
    //public interface ICoreWebView2Environment
    //{
    //    void CreateCoreWebView2Controller(
    //        IntPtr parentWindow,
    //        [MarshalAs(UnmanagedType.Interface)] ICreateCoreWebView2ControllerCompletedHandler handler);
    //}

    //[ComImport]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[Guid("6C4819F3-C9B7-4260-8127-C9F5BDE7F68C")]
    //public interface ICreateCoreWebView2ControllerCompletedHandler
    //{
    //    void Invoke([MarshalAs(UnmanagedType.Error)] int errorCode,
    //               [MarshalAs(UnmanagedType.Interface)] ICoreWebView2Controller? createdController);
    //}

    //[ComImport]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[Guid("4d00c0d1-9434-4eb6-8078-8697a560334f")]
    //public interface ICoreWebView2Controller
    //{
    //    ICoreWebView2 CoreWebView2 { [return: MarshalAs(UnmanagedType.Interface)] get; }
    //    void Close();
    //    IntPtr ParentWindow { get; set; }
    //    void get_Bounds(out tagRECT bounds);
    //    void put_Bounds(tagRECT bounds);
    //    int IsVisible { get; set; }
    //    void MoveFocus(COREWEBVIEW2_MOVE_FOCUS_REASON reason);
    //}

    //[ComImport]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[Guid("76eceacb-0462-4d94-ac83-423a6793775e")]
    //public interface ICoreWebView2
    //{
        //void Navigate([MarshalAs(UnmanagedType.LPWStr)] string uri);
        //void NavigateToString([MarshalAs(UnmanagedType.LPWStr)] string htmlContent);
        //void GoBack();
        //void GoForward();
        //void Reload();
        //void Stop();
        //void ExecuteScript(
        //    [MarshalAs(UnmanagedType.LPWStr)] string javaScript,
        //    [MarshalAs(UnmanagedType.Interface)] IExecuteScriptCompletedHandler handler);
        //int CanGoBack { get; }
        //int CanGoForward { get; }
        //string Source { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
        //void add_NavigationStarting(
        //    [MarshalAs(UnmanagedType.Interface)] INavigationStartingEventHandler eventHandler,
        //    out long token);
        //void remove_NavigationStarting(long token);
        //void add_NavigationCompleted(
        //    [MarshalAs(UnmanagedType.Interface)] INavigationCompletedEventHandler eventHandler,
        //    out long token);
        //void remove_NavigationCompleted(long token);
    //}

    //[ComImport]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[Guid("b8c10903-4e81-43f6-99c4-9e2b8c374c4a")]
    //public interface INavigationStartingEventHandler
    //{
    //    void Invoke([MarshalAs(UnmanagedType.Interface)] ICoreWebView2 sender,
    //               [MarshalAs(UnmanagedType.Interface)] ICoreWebView2NavigationStartingEventArgs args);
    //}

    //[ComImport]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[Guid("5b495469-e119-438a-9b18-7604f25f2e49")]
    //public interface INavigationCompletedEventHandler
    //{
    //    void Invoke([MarshalAs(UnmanagedType.Interface)] ICoreWebView2 sender,
    //               [MarshalAs(UnmanagedType.Interface)] ICoreWebView2NavigationCompletedEventArgs args);
    //}

    //[ComImport]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[Guid("9ADFB1A4-4BF3-4F8D-8F2A-B6D6F6C7F0D1")]
    //public interface ICoreWebView2NavigationStartingEventArgs
    //{
    //    string Uri { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
    //    int IsUserInitiated { get; }
    //    int Cancel { get; set; }
    //}

    //[ComImport]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[Guid("30d68b7d-20d9-4752-a9ca-ec8448fbb5c1")]
    //public interface ICoreWebView2NavigationCompletedEventArgs
    //{
    //    int IsSuccess { get; }
    //    COREWEBVIEW2_WEB_ERROR_STATUS WebErrorStatus { get; }
    //    long NavigationId { get; }
    //}

    //[ComImport]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[Guid("49511172-cc67-4bca-9923-137112f4c4cc")]
    //public interface IExecuteScriptCompletedHandler
    //{
    //    void Invoke([MarshalAs(UnmanagedType.Error)] int errorCode,
    //               [MarshalAs(UnmanagedType.LPWStr)] string resultObjectAsJson);
    //}

    // Enums
    //public enum COREWEBVIEW2_MOVE_FOCUS_REASON
    //{
    //    COREWEBVIEW2_MOVE_FOCUS_REASON_PROGRAMMATIC = 0,
    //    COREWEBVIEW2_MOVE_FOCUS_REASON_NEXT = 1,
    //    COREWEBVIEW2_MOVE_FOCUS_REASON_PREVIOUS = 2,
    //}

    //public enum COREWEBVIEW2_WEB_ERROR_STATUS
    //{
    //    COREWEBVIEW2_WEB_ERROR_STATUS_UNKNOWN = 0,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_CERTIFICATE_COMMON_NAME_IS_INCORRECT = 1,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_CERTIFICATE_EXPIRED = 2,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_CLIENT_CERTIFICATE_CONTAINS_ERRORS = 3,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_CERTIFICATE_REVOKED = 4,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_CERTIFICATE_IS_INVALID = 5,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_SERVER_UNREACHABLE = 6,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_TIMEOUT = 7,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_ERROR_HTTP_INVALID_SERVER_RESPONSE = 8,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_CONNECTION_ABORTED = 9,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_CONNECTION_RESET = 10,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_DISCONNECTED = 11,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_CANNOT_CONNECT = 12,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_HOST_NAME_NOT_RESOLVED = 13,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_OPERATION_CANCELED = 14,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_REDIRECT_FAILED = 15,
    //    COREWEBVIEW2_WEB_ERROR_STATUS_UNEXPECTED_ERROR = 16,
    //}

    // Structures
    //[StructLayout(LayoutKind.Sequential)]
    //public struct tagRECT
    //{
    //    public int left;
    //    public int top;
    //    public int right;
    //    public int bottom;
    //}

    // Window styles
    public const uint WS_CHILD = 0x40000000;
    public const uint WS_VISIBLE = 0x10000000;
    public const uint WS_CLIPCHILDREN = 0x02000000;
    public const uint WS_CLIPSIBLINGS = 0x04000000;

    // ShowWindow commands
    public const int SW_HIDE = 0;
    public const int SW_SHOW = 5;

    // SetWindowPos flags
    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_NOACTIVATE = 0x0010;
}
