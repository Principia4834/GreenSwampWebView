using System;
using System.Numerics;

namespace SampleRotator.Models;

/// <summary>
/// Represents the configuration state of a 3D model
/// </summary>
public class ModelConfiguration
{
    /// <summary>
    /// Configuration origin position
    /// </summary>
    public Vector3 Origin { get; set; } = Vector3.Zero;

    /// <summary>
    /// Configuration orientation in degrees (Theta, Phi)
    /// </summary>
    public Vector2 ConfigOrientation { get; set; } = Vector2.Zero;

    /// <summary>
    /// Current rotation in degrees (Theta, Phi)
    /// </summary>
    public Vector2 Rotation { get; set; } = Vector2.Zero;

    /// <summary>
    /// Path to the OBJ file
    /// </summary>
    public string? ObjFilePath { get; set; }

    /// <summary>
    /// Path to the MTL file (if available)
    /// </summary>
    public string? MtlFilePath { get; set; }

    /// <summary>
    /// Computes the combined transformation matrix
    /// </summary>
    public Matrix4x4 GetTransformationMatrix()
    {
        // Translation to origin
        var translation = Matrix4x4.CreateTranslation(Origin);

        // Configuration orientation rotation
        var configRotation = CreateRotationMatrix(
            ConfigOrientation.X, 
            ConfigOrientation.Y);

        // Current rotation
        var currentRotation = CreateRotationMatrix(
            Rotation.X, 
            Rotation.Y);

        // Combine transformations: Translate -> ConfigRotate -> CurrentRotate
        return translation * configRotation * currentRotation;
    }

    /// <summary>
    /// Creates a rotation matrix from theta and phi angles in degrees
    /// </summary>
    private static Matrix4x4 CreateRotationMatrix(double thetaDegrees, double phiDegrees)
    {
        // Convert degrees to radians
        float theta = (float)(thetaDegrees * Math.PI / 180.0);
        float phi = (float)(phiDegrees * Math.PI / 180.0);

        // Theta rotation around X-axis
        var rotX = Matrix4x4.CreateRotationX(theta);

        // Phi rotation around Y-axis
        var rotY = Matrix4x4.CreateRotationY(phi);

        // Combined rotation
        return rotX * rotY;
    }
}
