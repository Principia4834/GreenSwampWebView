using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using GreenSwampWebView;

namespace WebViewDemo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        UpdateNavigationButtons();
    }

    private void BackButton_Click(object? sender, RoutedEventArgs e)
    {
        WebViewControl.GoBack();
        UpdateNavigationButtons();
    }

    private void ForwardButton_Click(object? sender, RoutedEventArgs e)
    {
        WebViewControl.GoForward();
        UpdateNavigationButtons();
    }

    private void ReloadButton_Click(object? sender, RoutedEventArgs e)
    {
        WebViewControl.Reload();
    }

    private void StopButton_Click(object? sender, RoutedEventArgs e)
    {
        WebViewControl.Stop();
    }

    private void NavigateButton_Click(object? sender, RoutedEventArgs e)
    {
        var url = UrlTextBox.Text;
        if (!string.IsNullOrWhiteSpace(url))
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }
            WebViewControl.Navigate(url);
        }
    }

    private void UrlTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            NavigateButton_Click(sender, new RoutedEventArgs());
        }
    }

    private void LoadHtmlButton_Click(object? sender, RoutedEventArgs e)
    {
        var html = @"
<!DOCTYPE html>
<html>
<head>
    <title>GreenSwamp WebView Demo</title>
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }
        .container {
            text-align: center;
            padding: 40px;
            background: rgba(255, 255, 255, 0.1);
            border-radius: 20px;
            backdrop-filter: blur(10px);
        }
        h1 {
            font-size: 3em;
            margin-bottom: 20px;
        }
        p {
            font-size: 1.5em;
            margin-bottom: 30px;
        }
        button {
            background: white;
            color: #667eea;
            border: none;
            padding: 15px 30px;
            font-size: 1.2em;
            border-radius: 10px;
            cursor: pointer;
            transition: transform 0.2s;
        }
        button:hover {
            transform: scale(1.05);
        }
    </style>
</head>
<body>
    <div class='container'>
        <h1>🌿 GreenSwamp WebView</h1>
        <p>Cross-Platform WebView Control for Avalonia UI</p>
        <p>✓ Windows (WebView2) • ✓ macOS (WKWebView) • ✓ Linux (WebKitGTK)</p>
        <button onclick=""alert('JavaScript works!')"">Test JavaScript</button>
    </div>
</body>
</html>";
        WebViewControl.NavigateToString(html);
    }

    private void WebView_NavigationStarting(object? sender, WebViewNavigationEventArgs e)
    {
        StatusTextBlock.Text = $"Navigating to: {e.Uri}";
        UpdateNavigationButtons();
    }

    private void WebView_NavigationCompleted(object? sender, WebViewNavigationEventArgs e)
    {
        if (e.IsSuccess)
        {
            StatusTextBlock.Text = $"Loaded: {e.Uri}";
        }
        else
        {
            StatusTextBlock.Text = $"Failed to load: {e.Uri}";
        }
        UpdateNavigationButtons();
    }

    private void WebView_NavigationFailed(object? sender, WebViewNavigationEventArgs e)
    {
        StatusTextBlock.Text = $"Navigation failed: {e.ErrorMessage}";
    }

    private void UpdateNavigationButtons()
    {
        BackButton.IsEnabled = WebViewControl.CanGoBack;
        ForwardButton.IsEnabled = WebViewControl.CanGoForward;
    }
}