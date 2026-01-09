using CommunityToolkit.Mvvm.ComponentModel;

namespace SampleRotator.ViewModels;

public partial class MainViewModel : ObservableObject
{
    // Configuration Origin
    [ObservableProperty]
    private double _originX = 0.0;

    [ObservableProperty]
    private double _originY = 0.0;

    [ObservableProperty]
    private double _originZ = 0.0;

    // Configuration Orientation
    [ObservableProperty]
    private double _configTheta = 0.0;

    [ObservableProperty]
    private double _configPhi = 0.0;

    // Model Rotation
    [ObservableProperty]
    private double _rotationTheta = 0.0;

    [ObservableProperty]
    private double _rotationPhi = 0.0;

    // Model Information
    [ObservableProperty]
    private string _modelFileName = "No model loaded";

    [ObservableProperty]
    private bool _isModelLoaded = false;
}
