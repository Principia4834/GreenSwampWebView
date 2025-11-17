# GreenSwamp WebView Control

A cross-platform WebView control for Avalonia UI that provides native web rendering on Windows, macOS, and Linux.

## Features

- ✅ **Cross-Platform Support**: Works on Windows, macOS, and Linux
- ✅ **Native Rendering Engines**:
  - Windows: WebView2 (Microsoft Edge Chromium)
  - macOS: WKWebView (WebKit)
  - Linux: WebKitGTK
- ✅ **Unified API**: Single API across all platforms
- ✅ **Full Navigation Control**: Back, forward, reload, and stop
- ✅ **JavaScript Execution**: Execute JavaScript code and get results
- ✅ **HTML String Support**: Load HTML content directly
- ✅ **Event Support**: Navigation events for tracking page loads

## Requirements

### Common Requirements
- .NET 8.0 SDK or later
- Avalonia UI 11.x

### Platform-Specific Requirements

#### Windows
- **Microsoft Edge WebView2 Runtime**
  - Usually pre-installed on Windows 10/11
  - Download from: https://developer.microsoft.com/en-us/microsoft-edge/webview2/

#### macOS
- macOS 10.13 (High Sierra) or later
- WebKit framework (included with macOS)

#### Linux
- WebKitGTK 4.0 or later
- Install on Ubuntu/Debian:
  ```bash
  sudo apt-get install libwebkit2gtk-4.0-dev
  ```
- Install on Fedora/RHEL:
  ```bash
  sudo dnf install webkit2gtk3-devel
  ```
- Install on Arch Linux:
  ```bash
  sudo pacman -S webkit2gtk
  ```

## Installation

### Using the Library in Your Project

1. Add a reference to the `GreenSwampWebView` project:
   ```xml
   <ItemGroup>
     <ProjectReference Include="path/to/WebViewControl/WebViewControl.csproj" />
   </ItemGroup>
   ```

2. Add the namespace to your AXAML file:
   ```xml
   xmlns:gsw="using:GreenSwampWebView"
   ```

3. Add the WebView control to your UI:
   ```xml
   <gsw:WebView Name="WebViewControl" Url="https://www.example.com" />
   ```

## Usage

### Basic Usage in AXAML

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:gsw="using:GreenSwampWebView"
        x:Class="YourApp.MainWindow">
    
    <gsw:WebView Name="MyWebView"
                 Url="https://www.google.com"
                 NavigationStarting="WebView_NavigationStarting"
                 NavigationCompleted="WebView_NavigationCompleted"
                 NavigationFailed="WebView_NavigationFailed" />
</Window>
```

### Code-Behind Usage

```csharp
using GreenSwampWebView;

// Navigate to a URL
webView.Navigate("https://www.example.com");

// Navigate to HTML content
webView.NavigateToString("<h1>Hello, WebView!</h1>");

// Navigation controls
webView.GoBack();
webView.GoForward();
webView.Reload();
webView.Stop();

// Check navigation state
bool canGoBack = webView.CanGoBack;
bool canGoForward = webView.CanGoForward;

// Execute JavaScript
string result = await webView.ExecuteScriptAsync("document.title");
```

### Event Handlers

```csharp
private void WebView_NavigationStarting(object? sender, WebViewNavigationEventArgs e)
{
    Console.WriteLine($"Starting navigation to: {e.Uri}");
}

private void WebView_NavigationCompleted(object? sender, WebViewNavigationEventArgs e)
{
    if (e.IsSuccess)
    {
        Console.WriteLine($"Successfully loaded: {e.Uri}");
    }
}

private void WebView_NavigationFailed(object? sender, WebViewNavigationEventArgs e)
{
    Console.WriteLine($"Navigation failed: {e.ErrorMessage}");
}
```

## API Reference

### WebView Control

#### Properties

- `string? Url` - Gets or sets the URL to navigate to
- `string? Html` - Gets or sets the HTML content to display
- `bool CanGoBack` - Gets whether the WebView can navigate back
- `bool CanGoForward` - Gets whether the WebView can navigate forward

#### Methods

- `void Navigate(string url)` - Navigates to the specified URL
- `void NavigateToString(string html)` - Loads the specified HTML content
- `void GoBack()` - Navigates back in history
- `void GoForward()` - Navigates forward in history
- `void Reload()` - Reloads the current page
- `void Stop()` - Stops loading the current page
- `Task<string> ExecuteScriptAsync(string script)` - Executes JavaScript code

#### Events

- `EventHandler<WebViewNavigationEventArgs> NavigationStarting` - Occurs when navigation starts
- `EventHandler<WebViewNavigationEventArgs> NavigationCompleted` - Occurs when navigation completes
- `EventHandler<WebViewNavigationEventArgs> NavigationFailed` - Occurs when navigation fails

### WebViewNavigationEventArgs

Properties:
- `string? Uri` - The URI of the navigation
- `bool IsSuccess` - Whether the navigation was successful
- `string? ErrorMessage` - Error message if navigation failed
- `int StatusCode` - HTTP status code

## Sample Application

A complete sample application is included in the `samples/WebViewDemo` directory. To run it:

```bash
cd samples/WebViewDemo
dotnet run
```

The demo application includes:
- URL navigation with address bar
- Back/Forward navigation buttons
- Reload and Stop buttons
- HTML content loading example
- Status bar showing navigation state

## Building from Source

```bash
# Clone the repository
git clone https://github.com/Principia4834/GreenSwampWebView.git
cd GreenSwampWebView

# Build the solution
dotnet build GS.WebViewControl.sln

# Run the sample
cd samples/WebViewDemo
dotnet run
```

## Architecture

The WebView control uses a platform abstraction pattern:

```
WebView (Main Control)
    ↓
IWebViewPlatform (Interface)
    ↓
├── WindowsWebView (Windows/WebView2)
├── MacOSWebView (macOS/WKWebView)
└── LinuxWebView (Linux/WebKitGTK)
```

Each platform implementation handles native interop through P/Invoke, abstracting platform differences behind a unified interface.

## Known Limitations

### Windows
- Requires WebView2 Runtime to be installed
- COM threading requirements are handled internally
- Async initialization may cause slight delay on first load

### macOS
- JavaScript execution is simplified (async callback not fully implemented)
- Coordinate system differences are handled automatically

### Linux
- Requires GTK and WebKitGTK libraries
- JavaScript execution doesn't return results (callback not implemented)
- Best tested on X11; Wayland support may vary

## Troubleshooting

### Windows: "WebView2 runtime not found"
- Install the WebView2 Runtime from Microsoft's website
- Or include the runtime with your application

### Linux: "libwebkit2gtk-4.0.so.37: cannot open shared object file"
- Install WebKitGTK: `sudo apt-get install libwebkit2gtk-4.0-37`

### macOS: WebView not appearing
- Ensure your application has proper window handles
- Check that macOS version is 10.13 or later

### General: WebView appears blank
- Check that the URL is valid and accessible
- Verify network connectivity
- Look for errors in the console/debug output

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Avalonia UI team for the excellent cross-platform framework
- Microsoft for WebView2
- Apple for WKWebView
- WebKitGTK team for the Linux implementation

## References

- [Avalonia UI Documentation](https://docs.avaloniaui.net/)
- [Microsoft WebView2 Documentation](https://docs.microsoft.com/en-us/microsoft-edge/webview2/)
- [WebKitGTK Documentation](https://webkitgtk.org/)
- [WKWebView Documentation](https://developer.apple.com/documentation/webkit/wkwebview)
