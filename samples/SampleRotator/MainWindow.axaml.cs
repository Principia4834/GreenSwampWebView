using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using GreenSwampWebView;
using SampleRotator.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SampleRotator;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private string? _currentObjFilePath;
    private bool _isViewerInitialized;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        DataContext = _viewModel;

        // Load the viewer HTML
        var htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "viewer.html");
        if (File.Exists(htmlPath))
        {
            WebViewControl.Navigate($"file:///{htmlPath.Replace("\\", "/")}");
        }
    }

    private void WebView_NavigationStarting(object? sender, WebViewNavigationEventArgs e)
    {
        StatusTextBlock.Text = $"Initializing 3D viewer...";
    }

    private async void WebView_NavigationCompleted(object? sender, WebViewNavigationEventArgs e)
    {
        if (e.IsSuccess)
        {
            StatusTextBlock.Text = "3D viewer loaded successfully";
            await InitializeBabylonViewerAsync();
        }
        else
        {
            StatusTextBlock.Text = "Failed to load 3D viewer";
        }
    }

    private void WebView_NavigationFailed(object? sender, WebViewNavigationEventArgs e)
    {
        StatusTextBlock.Text = $"Navigation failed: {e.ErrorMessage}";
    }

    private async Task InitializeBabylonViewerAsync()
    {
        try
        {
            // Initialize the Babylon.js viewer (will be implemented in Phase 2)
            var result = await WebViewControl.ExecuteScriptAsync("typeof initViewer === 'function' ? initViewer() : 'Not ready'");
            _isViewerInitialized = result.Contains("initialized") || result.Contains("Viewer");
            
            if (_isViewerInitialized)
            {
                StatusTextBlock.Text = "Babylon.js viewer initialized. Load a 3D model to begin.";
            }
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"Viewer initialization error: {ex.Message}";
        }
    }

    private async void LoadModelButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var storage = StorageProvider;
            
            var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select 3D Model (OBJ)",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("3D Model Files")
                    {
                        Patterns = new[] { "*.obj" },
                        MimeTypes = new[] { "model/obj", "application/octet-stream" }
                    },
                    FilePickerFileTypes.All
                }
            });

            if (files.Count > 0)
            {
                var file = files[0];
                _currentObjFilePath = file.Path.LocalPath;
                
                ModelFileNameText.Text = Path.GetFileName(_currentObjFilePath);
                StatusTextBlock.Text = $"Loading model: {Path.GetFileName(_currentObjFilePath)}";

                // Load the model (will be implemented in Phase 3)
                await LoadModelAsync(_currentObjFilePath);
            }
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"Error loading model: {ex.Message}";
        }
    }

    private async Task LoadModelAsync(string objFilePath)
    {
        try
        {
            if (!_isViewerInitialized)
            {
                StatusTextBlock.Text = "Error: Viewer not initialized";
                return;
            }

            if (!File.Exists(objFilePath))
            {
                StatusTextBlock.Text = "Error: OBJ file not found";
                return;
            }

            StatusTextBlock.Text = "Reading OBJ file...";

            // Read OBJ file content
            byte[] objBytes = await File.ReadAllBytesAsync(objFilePath);
            string objBase64 = Convert.ToBase64String(objBytes);

            // Check for accompanying MTL file
            string? mtlBase64 = null;
            string mtlFilePath = Path.ChangeExtension(objFilePath, ".mtl");
            
            if (File.Exists(mtlFilePath))
            {
                StatusTextBlock.Text = "Reading MTL file...";
                byte[] mtlBytes = await File.ReadAllBytesAsync(mtlFilePath);
                mtlBase64 = Convert.ToBase64String(mtlBytes);
            }

            StatusTextBlock.Text = "Transferring model to 3D viewer...";

            // Call JavaScript to load the model (wrap in async IIFE to get Promise result)
            string script = mtlBase64 != null
                ? $"(async () => {{ return await loadModel('{objBase64}', '{mtlBase64}'); }})()"
                : $"(async () => {{ return await loadModel('{objBase64}', null); }})()";

            System.Diagnostics.Debug.WriteLine("Executing model load script...");
            var result = await WebViewControl.ExecuteScriptAsync(script);
            System.Diagnostics.Debug.WriteLine($"Load result: {result}");

            // Check result
            if (string.IsNullOrEmpty(result))
            {
                StatusTextBlock.Text = "Warning: No response from viewer";
                _viewModel.IsModelLoaded = true; // Assume success if no error
            }
            else if (result.Contains("success", StringComparison.OrdinalIgnoreCase))
            {
                StatusTextBlock.Text = $"Model loaded: {Path.GetFileName(objFilePath)}";
                _viewModel.IsModelLoaded = true;
                _viewModel.ModelFileName = Path.GetFileName(objFilePath);

                // Reset transformation parameters to defaults
                _viewModel.OriginX = 0;
                _viewModel.OriginY = 0;
                _viewModel.OriginZ = 0;
                _viewModel.ConfigTheta = 0;
                _viewModel.ConfigPhi = 0;
                _viewModel.RotationTheta = 0;
                _viewModel.RotationPhi = 0;

                // Give JavaScript time to finish before applying transformations
                await Task.Delay(100);

                // Apply initial configuration state
                await UpdateModelOriginAsync();
                await UpdateConfigurationOrientationAsync();
            }
            else if (result.Contains("Error", StringComparison.OrdinalIgnoreCase))
            {
                StatusTextBlock.Text = $"Failed to load model: {result}";
                _viewModel.IsModelLoaded = false;
            }
            else
            {
                StatusTextBlock.Text = $"Model loaded: {Path.GetFileName(objFilePath)} (Response: {result})";
                _viewModel.IsModelLoaded = true;
                _viewModel.ModelFileName = Path.GetFileName(objFilePath);

                // Reset transformation parameters to defaults
                _viewModel.OriginX = 0;
                _viewModel.OriginY = 0;
                _viewModel.OriginZ = 0;
                _viewModel.ConfigTheta = 0;
                _viewModel.ConfigPhi = 0;
                _viewModel.RotationTheta = 0;
                _viewModel.RotationPhi = 0;

                // Give JavaScript time to finish before applying transformations
                await Task.Delay(100);

                // Apply initial configuration state
                await UpdateModelOriginAsync();
                await UpdateConfigurationOrientationAsync();
            }
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"Error loading model: {ex.Message}";
            _viewModel.IsModelLoaded = false;
        }
    }

    private async void OriginValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_isViewerInitialized || !_viewModel.IsModelLoaded) return;

        // Apply origin transformation
        await UpdateModelOriginAsync();
    }

    private async void ConfigOrientationChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_isViewerInitialized || !_viewModel.IsModelLoaded) return;

        // Apply configuration orientation
        await UpdateConfigurationOrientationAsync();
    }

    private async void RotationSliderChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (!_isViewerInitialized || !_viewModel.IsModelLoaded) return;

        // Apply rotation transformation
        await UpdateModelRotationAsync();
    }

    private void ResetRotationButton_Click(object? sender, RoutedEventArgs e)
    {
        _viewModel.RotationTheta = 0;
        _viewModel.RotationPhi = 0;
        
        // The slider ValueChanged event will trigger UpdateModelRotationAsync automatically
    }

    private async Task UpdateModelOriginAsync()
    {
        try
        {
            if (!_viewModel.IsModelLoaded)
            {
                return;
            }

            // Get origin values from ViewModel
            double x = _viewModel.OriginX;
            double y = _viewModel.OriginY;
            double z = _viewModel.OriginZ;

            // Call JavaScript to update origin (use invariant culture for decimal point)
            string script = $"setOrigin({x.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {y.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {z.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
            System.Diagnostics.Debug.WriteLine($"Calling: {script}");
            var result = await WebViewControl.ExecuteScriptAsync(script);
            System.Diagnostics.Debug.WriteLine($"Result: {result}");

            StatusTextBlock.Text = $"Origin: ({x:F2}, {y:F2}, {z:F2})";
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"Error updating origin: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error in UpdateModelOriginAsync: {ex}");
        }
    }

    private async Task UpdateConfigurationOrientationAsync()
    {
        try
        {
            if (!_viewModel.IsModelLoaded)
            {
                return;
            }

            // Get configuration orientation values from ViewModel
            double theta = _viewModel.ConfigTheta;
            double phi = _viewModel.ConfigPhi;

            // Call JavaScript to update configuration orientation (use invariant culture for decimal point)
            string script = $"setConfigOrientation({theta.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {phi.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
            System.Diagnostics.Debug.WriteLine($"Calling: {script}");
            var result = await WebViewControl.ExecuteScriptAsync(script);
            System.Diagnostics.Debug.WriteLine($"Result: {result}");

            StatusTextBlock.Text = $"Config Orientation: ?={theta:F1}°, ?={phi:F1}°";
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"Error updating orientation: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error in UpdateConfigurationOrientationAsync: {ex}");
        }
    }

    private async Task UpdateModelRotationAsync()
    {
        try
        {
            if (!_viewModel.IsModelLoaded)
            {
                return;
            }

            // Get rotation values from ViewModel
            double theta = _viewModel.RotationTheta;
            double phi = _viewModel.RotationPhi;

            // Call JavaScript to update model rotation (use invariant culture for decimal point)
            string script = $"setRotation({theta.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {phi.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
            System.Diagnostics.Debug.WriteLine($"Calling: {script}");
            var result = await WebViewControl.ExecuteScriptAsync(script);
            System.Diagnostics.Debug.WriteLine($"Result: {result}");

            StatusTextBlock.Text = $"Rotation: ?={theta:F1}°, ?={phi:F1}°";
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"Error updating rotation: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error in UpdateModelRotationAsync: {ex}");
        }
    }
}
