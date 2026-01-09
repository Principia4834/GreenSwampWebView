**Specification for SampleRotator Application**

**Description**

SampleRotator is an application that demonstrates use of the GreenSwamp WebView Control to display and control a 3d view of an object.

The GreenSwamp WebView Control, [Principia4834/GreenSwampWebView](https://github.com/Principia4834/GreenSwampWebView) , has been developed to enable Avalonia applications, [Avalonia UI – Open-Source .NET XAML Framework | WPF & MAUI Alternative](https://avaloniaui.net/) to host html pages.

This sample demonstrates how load, configure, view and manipulate a 3D model using Babylon.js, [Babylon.js: Powerful, Beautiful, Simple, Open - Web-Based 3D At Its Best](https://www.babylonjs.com/) hosted in the WebView control.

**Functional Requirements**

1.  The user shall be able to load a 3D model from a “.obj” format file stored in the local file system. If a “.mtl” file is available that will also be loaded The model will be located at the (x,y,z) origin (0,0,0)
2.  After loading the user shall be able to configure the model origin and rotation axes
    1.  The user shall be able to translate the model to a configuration origin position given in (x,y,z) coordinates
    2.  The user shall be able rotate the model to a new a configuration axes orientation given in (theta, phi) coordinates
3.  The user shall be able to rotate the model about the configuration origin using (theta, phi) coordinates referenced to the configuration origin

**User Interface**

1.  The UI will have a 3D viewport panel and vertical side panel
2.  The 3D viewport will enable control of the view using the mouse to move, scale and rotate the view
3.  The side panel will enable selection of a 3D model in “.obj” format using a file picker to find the file in the local file system
4.  The side panel will have enable entry of the configuration origin, with default values of (0,0,0)
5.  The side panel will have enable entry of the configuration axes, with default values of (0,0)
6.  The side panel will have two slider controls to allow rotation of model about the configuration (x,y,z) and (theta,phi) origin.
7.  The theta model rotation shall be in the range -90 degrees to + 90 degrees
8.  The phi model rotation shall be in the range -180 degrees to + 180 degrees

**Implementation Stack**

The IDE is Visual Studio 2026.

The application will use the NET 8 runtime.

The Avalonia components will be provided by NuGet packages. Avalonia uses “.axaml” files to define the UI layout

The Babylon.js components will be provided by the Bablylon.js web site and the associated Git Hub repositories.

The WebView control will be used. Important: this control may require additional JavaScript to C# interfaces. If these are required they should be generic and capable of re-use.

All compute intensive calculations will be implemented in c# and made available to the web view and javascript as required.

**Solution and Project Structure**

The current solution has the source code for the WebView control in a project and a sample application in a second project. The SampleRotator application will be a separate project created under the same solution. The new application should take the current sample application as a template.

**Outline Implementation Plan**

The project will be implemented in multiple stages:

1.  Avalonia application “boiler-plate” implementing the window structure (viewport panel and configuration / control panel) to include all entry boxes, file picker etc not wired up to any functionality
2.  Web view setup with empty Babylon.js framework
3.  Loading and viewing of “.obj” file in viewport window, with Babylon/js native controls to manage the viewport using the mouse
4.  Wiring up of “.obj” configuration parameters so the origin and orientation of the “.obj” model can be set
5.  Wiring up of the (theta, phi) sliders so the model moves around the configurated origin