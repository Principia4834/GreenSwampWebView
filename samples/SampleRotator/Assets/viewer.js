// SampleRotator - Babylon.js 3D Viewer
// Phase 2: Complete Babylon.js Scene Integration

/**
 * Main BabylonViewer class - manages 3D scene, camera, lighting, and model
 */
class BabylonViewer {
    constructor(canvasId) {
        this.canvas = document.getElementById(canvasId);
        if (!this.canvas) {
            console.error('Canvas element not found:', canvasId);
            return;
        }

        this.engine = null;
        this.scene = null;
        this.camera = null;
        this.currentMesh = null;
        this.transformNode = null;  // Pivot point for rotations
        this.isInitialized = false;

        this.initialize();
    }

    /**
     * Initialize Babylon.js engine and scene
     */
    initialize() {
        try {
            // Create Babylon.js engine
            this.engine = new BABYLON.Engine(this.canvas, true, {
                preserveDrawingBuffer: true,
                stencil: true,
                antialias: true
            });

            // Create the scene
            this.scene = this.createScene();

            // Start render loop
            this.engine.runRenderLoop(() => {
                if (this.scene) {
                    // Update grid scale based on camera distance
                    this.updateGridScale();
                    
                    this.scene.render();
                }
            });

            // Handle window resize
            window.addEventListener('resize', () => {
                this.engine.resize();
            });

            this.isInitialized = true;
            this.hideLoadingMessage();
            console.log('Babylon.js viewer initialized successfully');

        } catch (error) {
            console.error('Failed to initialize Babylon.js viewer:', error);
            this.showError('Initialization failed: ' + error.message);
        }
    }

    /**
     * Create and configure the 3D scene
     */
    createScene() {
        const scene = new BABYLON.Scene(this.engine);

        // Set background color (dark gray)
        scene.clearColor = new BABYLON.Color4(0.15, 0.15, 0.15, 1.0);

        // Create ArcRotateCamera for mouse-controlled view
        // Parameters: name, alpha (horizontal), beta (vertical), radius, target, scene
        this.camera = new BABYLON.ArcRotateCamera(
            "camera",
            -Math.PI / 2,        // Start looking from the right
            Math.PI / 3,          // Angle from top
            10,                   // Distance from target
            BABYLON.Vector3.Zero(),
            scene
        );

        // Attach camera controls to canvas (enables mouse interaction)
        this.camera.attachControl(this.canvas, true);

        // Configure camera limits for large models
        this.camera.lowerRadiusLimit = 0.1;     // Allow very close zoom (for small details)
        this.camera.upperRadiusLimit = 10000;   // Allow very far zoom (for large objects)
        this.camera.wheelPrecision = 1;         // Finer mouse wheel control (lower = more sensitive)
        this.camera.panningSensibility = 50;    // Improved panning sensitivity
        
        // Enable inertia for smoother camera movement
        this.camera.inertia = 0.9;
        this.camera.angularSensibilityX = 1000;
        this.camera.angularSensibilityY = 1000;

        // Create lighting
        this.setupLighting(scene);

        // Add a grid helper for reference
        this.addGridHelper(scene);

        // Add Y-axis with tick marks
        this.addYAxis(scene);

        // Add XYZ axis indicator in corner
        this.addAxisIndicator(scene);

        return scene;
    }

    /**
     * Setup scene lighting
     */
    setupLighting(scene) {
        // Hemispheric light for ambient illumination
        const hemisphericLight = new BABYLON.HemisphericLight(
            "hemisphericLight",
            new BABYLON.Vector3(0, 1, 0),
            scene
        );
        hemisphericLight.intensity = 0.7;
        hemisphericLight.diffuse = new BABYLON.Color3(1, 1, 1);
        hemisphericLight.specular = new BABYLON.Color3(0.3, 0.3, 0.3);
        hemisphericLight.groundColor = new BABYLON.Color3(0.2, 0.2, 0.2);

        // Directional light for shadows and highlights
        const directionalLight = new BABYLON.DirectionalLight(
            "directionalLight",
            new BABYLON.Vector3(-1, -2, -1),
            scene
        );
        directionalLight.position = new BABYLON.Vector3(10, 20, 10);
        directionalLight.intensity = 0.5;
    }

    /**
     * Add a ground grid for spatial reference
     */
    addGridHelper(scene) {
        // Create ground plane with grid
        this.ground = BABYLON.MeshBuilder.CreateGround(
            "ground",
            { width: 20, height: 20, subdivisions: 20 },
            scene
        );

        // Create grid material
        const groundMaterial = new BABYLON.StandardMaterial("groundMaterial", scene);
        groundMaterial.diffuseColor = new BABYLON.Color3(0.3, 0.3, 0.3);
        groundMaterial.specularColor = new BABYLON.Color3(0.1, 0.1, 0.1);
        groundMaterial.wireframe = true;
        groundMaterial.alpha = 0.3;

        this.ground.material = groundMaterial;
        this.ground.position.y = -0.01;  // Slightly below origin
        this.ground.isPickable = false;   // Don't interfere with model picking
        
        // Store initial grid size for scaling calculations
        this.initialGridSize = 20;
    }

    /**
     * Add Y-axis with tick marks for height reference
     */
    addYAxis(scene) {
        // Create main Y-axis line (vertical)
        const axisHeight = 5000; // Extends +/- 5000 units
        const yAxisPoints = [
            new BABYLON.Vector3(0, -axisHeight, 0),
            new BABYLON.Vector3(0, axisHeight, 0)
        ];
        
        this.yAxis = BABYLON.MeshBuilder.CreateLines("yAxis", {
            points: yAxisPoints
        }, scene);
        this.yAxis.color = new BABYLON.Color3(0, 1, 0); // Green for Y-axis
        this.yAxis.isPickable = false;

        // Create tick marks every 200 units
        this.yAxisTicks = [];
        const tickInterval = 200;
        const tickSize = 50; // Length of tick mark
        
        for (let y = -axisHeight; y <= axisHeight; y += tickInterval) {
            if (y === 0) continue; // Skip origin (already shown by grid)
            
            // Horizontal tick mark
            const tickPoints = [
                new BABYLON.Vector3(-tickSize / 2, y, 0),
                new BABYLON.Vector3(tickSize / 2, y, 0)
            ];
            
            const tick = BABYLON.MeshBuilder.CreateLines(`yTick_${y}`, {
                points: tickPoints
            }, scene);
            tick.color = new BABYLON.Color3(0, 0.8, 0); // Slightly darker green
            tick.isPickable = false;
            
            this.yAxisTicks.push(tick);
        }

        console.log(`Y-axis created with ticks from ${-axisHeight} to ${axisHeight} every ${tickInterval} units`);
    }

    /**
     * Add XYZ axis indicator in bottom-right corner
     */
    addAxisIndicator(scene) {
        // Use Babylon.js built-in AxesViewer for world coordinate system
        // This automatically creates RGB axes at the origin
        this.axesViewer = new BABYLON.AxesViewer(scene, 100); // 100 = axis length in scene units
        
        // The axes are positioned at world origin (0,0,0)
        // Red = X, Green = Y, Blue = Z (Babylon.js standard colors)
        
        console.log('XYZ axes viewer created at world origin (Red=X, Green=Y, Blue=Z)');
    }

    /**
     * Update grid scale based on camera distance
     * Call this during camera movement to maintain ~20 visible grid lines
     */
    updateGridScale() {
        if (!this.ground || !this.camera) return;

        // Calculate appropriate grid size based on camera radius
        // Goal: Keep ~20 grid lines visible in the viewport
        // The visible area is roughly proportional to camera.radius
        const targetGridSize = this.camera.radius * 1.5;
        
        // Calculate scale factor
        const scale = targetGridSize / this.initialGridSize;
        
        // Apply uniform scale to ground grid
        this.ground.scaling = new BABYLON.Vector3(scale, 1, scale);
    }

    /**
     * Load a 3D model from base64-encoded OBJ data
     * @param {string} objDataBase64 - Base64-encoded OBJ file content
     * @param {string|null} mtlDataBase64 - Base64-encoded MTL file content (optional)
     * @returns {Promise<string>} - Success or error message
     */
    async loadModel(objDataBase64, mtlDataBase64) {
        try {
            if (!this.isInitialized) {
                throw new Error('Viewer not initialized');
            }

            // Clear existing model
            if (this.currentMesh) {
                this.currentMesh.dispose();
                this.currentMesh = null;
            }
            if (this.transformNode) {
                this.transformNode.dispose();
                this.transformNode = null;
            }

            // Convert base64 to blob and create object URL
            const objBlob = this.base64ToBlob(objDataBase64);
            const objUrl = URL.createObjectURL(objBlob);

            console.log('Loading OBJ model...');

            // Load model using Babylon.js SceneLoader
            const result = await BABYLON.SceneLoader.ImportMeshAsync(
                "",           // Import all meshes
                "",           // Root URL (not used with object URL)
                objUrl,       // File URL
                this.scene,
                null,         // onProgress callback
                ".obj"        // File extension
            );

            // Clean up object URL
            URL.revokeObjectURL(objUrl);

            if (result.meshes.length === 0) {
                throw new Error('No meshes found in OBJ file');
            }

            console.log(`Loaded ${result.meshes.length} mesh(es)`);

            // Create a parent transform node for pivot-based rotation
            this.transformNode = new BABYLON.TransformNode("pivotNode", this.scene);

            // Parent all loaded meshes to the transform node
            result.meshes.forEach(mesh => {
                if (mesh.parent === null) {
                    mesh.parent = this.transformNode;
                }
            });

            this.currentMesh = result.meshes[0];

            // Center camera on model
            this.focusOnModel();

            return "Model loaded successfully";

        } catch (error) {
            console.error('Failed to load model:', error);
            return `Error loading model: ${error.message}`;
        }
    }

    /**
     * Focus camera on the loaded model
     */
    focusOnModel() {
        if (!this.transformNode || !this.scene) return;

        // Get only the model meshes (exclude ground, axis, and their children)
        const modelMeshes = this.scene.meshes.filter(mesh => {
            // Exclude non-mesh objects and helper objects
            if (!mesh.getTotalVertices || mesh.getTotalVertices() === 0) return false;
            
            // Exclude named helper objects
            if (mesh.name === 'ground' || 
                mesh.name === 'yAxis' || 
                mesh.name.startsWith('yTick_')) {
                return false;
            }
            
            // Include meshes that are children of transformNode
            let parent = mesh.parent;
            while (parent) {
                if (parent === this.transformNode) return true;
                parent = parent.parent;
            }
            
            return false;
        });

        if (modelMeshes.length === 0) {
            console.warn('No model meshes found for camera focusing');
            return;
        }

        console.log(`Focusing on ${modelMeshes.length} model mesh(es)`);

        // Calculate bounding box of model meshes only
        let min = modelMeshes[0].getBoundingInfo().boundingBox.minimumWorld.clone();
        let max = modelMeshes[0].getBoundingInfo().boundingBox.maximumWorld.clone();

        for (let i = 1; i < modelMeshes.length; i++) {
            const meshMin = modelMeshes[i].getBoundingInfo().boundingBox.minimumWorld;
            const meshMax = modelMeshes[i].getBoundingInfo().boundingBox.maximumWorld;
            min = BABYLON.Vector3.Minimize(min, meshMin);
            max = BABYLON.Vector3.Maximize(max, meshMax);
        }

        const center = BABYLON.Vector3.Center(min, max);
        const size = max.subtract(min).length();

        console.log(`Model bounding box: min=(${min.x.toFixed(1)}, ${min.y.toFixed(1)}, ${min.z.toFixed(1)}), max=(${max.x.toFixed(1)}, ${max.y.toFixed(1)}, ${max.z.toFixed(1)})`);
        console.log(`Model center: (${center.x.toFixed(1)}, ${center.y.toFixed(1)}, ${center.z.toFixed(1)}), diagonal size: ${size.toFixed(1)}`);

        // Update camera target and radius
        // Use 2.5x multiplier for better initial view of large models
        this.camera.target = center;
        this.camera.radius = size * 2.5;
        
        // Adjust camera limits based on model size
        this.camera.lowerRadiusLimit = size * 0.01;  // 1% of model size
        this.camera.upperRadiusLimit = size * 20;     // 20x model size (increased from 10x)
        
        // Reset camera angles to default view
        this.camera.alpha = -Math.PI / 2;    // Side view
        this.camera.beta = Math.PI / 3;      // 60 degrees from top
        
        // Update grid scale to match new camera position
        this.updateGridScale();
        
        console.log(`Camera positioned: target=(${center.x.toFixed(1)}, ${center.y.toFixed(1)}, ${center.z.toFixed(1)}), radius=${this.camera.radius.toFixed(1)}`);
        console.log(`Zoom limits: min=${this.camera.lowerRadiusLimit.toFixed(1)}, max=${this.camera.upperRadiusLimit.toFixed(1)}`);
    }

    /**
     * Set the configuration origin (translation)
     * @param {number} x - X coordinate
     * @param {number} y - Y coordinate
     * @param {number} z - Z coordinate
     */
    setOrigin(x, y, z) {
        if (!this.transformNode) {
            console.warn('No model loaded - cannot set origin');
            return;
        }

        // Validate inputs - ensure they are numbers
        x = parseFloat(x) || 0;
        y = parseFloat(y) || 0;
        z = parseFloat(z) || 0;

        // Check for valid numbers (not NaN or Infinity)
        if (!isFinite(x) || !isFinite(y) || !isFinite(z)) {
            console.error(`Invalid origin values: x=${x}, y=${y}, z=${z}`);
            return;
        }

        this.transformNode.position = new BABYLON.Vector3(x, y, z);
        console.log(`? Origin set to: (${x}, ${y}, ${z})`);
        console.log(`Transform node position:`, this.transformNode.position);
    }

    /**
     * Set configuration orientation (base rotation)
     * @param {number} thetaDegrees - Theta angle in degrees (angle from Y-axis, 0° = pointing up)
     * @param {number} phiDegrees - Phi angle in degrees (rotation in XZ plane, 0° = +X axis)
     */
    setConfigOrientation(thetaDegrees, phiDegrees) {
        if (!this.transformNode) {
            console.warn('No model loaded - cannot set orientation');
            return;
        }

        // Convert degrees to radians
        const thetaRad = BABYLON.Angle.FromDegrees(thetaDegrees).radians();
        const phiRad = BABYLON.Angle.FromDegrees(phiDegrees).radians();

        // NEW CONVENTION:
        // theta: angle from Y-axis (0° = pointing up along +Y, 90° = horizontal in XZ plane)
        // phi: rotation in XZ plane (0° = +X direction, 90° = +Z direction)
        //
        // Starting point: telescope pointing up (+Y axis)
        // 1. Rotate around Y-axis by phi (sets azimuth in XZ plane)
        // 2. Tilt away from Y-axis by theta (sets elevation)
        //
        // To implement:
        // - Y rotation = phi (azimuth in XZ plane)
        // - X rotation = theta (tilt from Y-axis)
        // - Z rotation = 0 (no roll)
        
        const xRotation = 0;             // No roll (tube doesn't spin around itself)
        const yRotation = phiRad;        // Horizontal rotation (azimuth in XZ)
        const zRotation = thetaRad;      // Tilt up/down (lifts tube from XY plane)
        
        this.transformNode.rotation = new BABYLON.Vector3(xRotation, yRotation, zRotation);
        
        console.log(`? Config orientation set to: theta=${thetaDegrees}° (from Y-axis), phi=${phiDegrees}° (in XZ plane)`);
        console.log(`  Euler angles: X=${thetaDegrees}°, Y=${phiDegrees}°, Z=0°`);
        console.log(`Transform node rotation:`, this.transformNode.rotation);
    }

    /**
     * Set model rotation around configured origin
     * @param {number} thetaDegrees - Theta rotation in degrees (additional angle from Y-axis)
     * @param {number} phiDegrees - Phi rotation in degrees (additional rotation in XZ plane)
     */
    setRotation(thetaDegrees, phiDegrees) {
        if (!this.currentMesh) {
            console.warn('No model loaded');
            return;
        }

        // Convert degrees to radians
        const thetaRad = BABYLON.Angle.FromDegrees(thetaDegrees).radians();
        const phiRad = BABYLON.Angle.FromDegrees(phiDegrees).radians();

        // Same convention as setConfigOrientation:
        // theta: angle from Y-axis
        // phi: rotation in XZ plane
        
        const xRotation = 0;             // No roll (tube doesn't spin around itself)
        const yRotation = phiRad;        // Horizontal rotation (azimuth in XZ)
        const zRotation = thetaRad;      // Tilt up/down (lifts tube from XY plane)
        
        this.currentMesh.rotation = new BABYLON.Vector3(xRotation, yRotation, zRotation);
        
        console.log(`Rotation set to: theta=${thetaDegrees}° (tilt up/down), phi=${phiDegrees}° (horizontal rotation)`);
    }

    /**
     * Convert base64 string to Blob
     * @param {string} base64 - Base64 encoded string
     * @returns {Blob} - Blob object
     */
    base64ToBlob(base64) {
        const binaryString = atob(base64);
        const bytes = new Uint8Array(binaryString.length);
        for (let i = 0; i < binaryString.length; i++) {
            bytes[i] = binaryString.charCodeAt(i);
        }
        return new Blob([bytes]);
    }

    /**
     * Hide loading message
     */
    hideLoadingMessage() {
        const loadingMsg = document.getElementById('loadingMessage');
        if (loadingMsg) {
            loadingMsg.classList.add('hidden');
        }
    }

    /**
     * Show error message
     */
    showError(message) {
        const loadingMsg = document.getElementById('loadingMessage');
        if (loadingMsg) {
            loadingMsg.innerHTML = `<p style="color: #ff6b6b;">?? Error</p><p style="font-size: 14px;">${message}</p>`;
        }
    }

    /**
     * Dispose of all resources
     */
    dispose() {
        if (this.scene) {
            this.scene.dispose();
        }
        if (this.engine) {
            this.engine.dispose();
        }
    }
}

// Global viewer instance
let viewer = null;

/**
 * Initialize the Babylon.js viewer
 * Called by C# after page load
 */
function initViewer() {
    try {
        if (typeof BABYLON === 'undefined') {
            throw new Error('Babylon.js library not loaded');
        }

        viewer = new BabylonViewer('renderCanvas');
        console.log('? SampleRotator viewer initialized');
        return "Viewer initialized";

    } catch (error) {
        console.error('Failed to initialize viewer:', error);
        return `Error: ${error.message}`;
    }
}

// Auto-initialize when page loads
window.addEventListener('load', function() {
    console.log('SampleRotator viewer.js loaded');
    initViewer();
});

// ============================================================================
// C# Bridge Functions - Called from C# via ExecuteScriptAsync
// ============================================================================

/**
 * Load a 3D model from base64 data
 * @param {string} objDataBase64 - Base64-encoded OBJ file
 * @param {string|null} mtlDataBase64 - Base64-encoded MTL file (optional)
 * @returns {Promise<string>} - Result message
 */
async function loadModel(objDataBase64, mtlDataBase64) {
    if (!viewer) {
        return "Error: Viewer not initialized";
    }
    return await viewer.loadModel(objDataBase64, mtlDataBase64);
}

/**
 * Set configuration origin
 * @param {number} x - X coordinate
 * @param {number} y - Y coordinate
 * @param {number} z - Z coordinate
 */
function setOrigin(x, y, z) {
    console.log(`C# called setOrigin(${x}, ${y}, ${z})`);
    if (viewer) {
        viewer.setOrigin(x, y, z);
        return `Origin set to (${x}, ${y}, ${z})`;
    } else {
        console.error('Viewer not initialized');
        return "Error: Viewer not initialized";
    }
}

/**
 * Set configuration orientation
 * @param {number} theta - Theta angle in degrees
 * @param {number} phi - Phi angle in degrees
 */
function setConfigOrientation(theta, phi) {
    console.log(`C# called setConfigOrientation(${theta}, ${phi})`);
    if (viewer) {
        viewer.setConfigOrientation(theta, phi);
        return `Config orientation set to theta=${theta}°, phi=${phi}°`;
    } else {
        console.error('Viewer not initialized');
        return "Error: Viewer not initialized";
    }
}

/**
 * Set model rotation
 * @param {number} theta - Theta rotation in degrees
 * @param {number} phi - Phi rotation in degrees
 */
function setRotation(theta, phi) {
    console.log(`C# called setRotation(${theta}, ${phi})`);
    if (viewer) {
        viewer.setRotation(theta, phi);
        return `Rotation set to theta=${theta}°, phi=${phi}°`;
    } else {
        console.error('Viewer not initialized');
        return "Error: Viewer not initialized";
    }
}

/**
 * Reset camera to default view
 */
function resetCamera() {
    if (viewer && viewer.camera) {
        viewer.camera.alpha = -Math.PI / 2;
        viewer.camera.beta = Math.PI / 3;
        viewer.focusOnModel();
    }
}

