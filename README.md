# 🎮 UltimateCameraCuller

**UltimateCameraCuller** is an ultra-fast, GPU-accelerated frustum culling library for .NET game engines. 

Instead of wasting precious CPU cycles iterating through hundreds of thousands of objects, this library leverages the GPU to calculate mathematical visibility instantly. It uses advanced **Clip Space Math** to return a simple `bool[]` telling your game engine exactly what to render and what to hide.

Powered by the lightning-fast [FastMatrixEngine](https://www.nuget.org/packages/FastMatrixEngine/), this culler ensures your game runs at a buttery-smooth 120+ FPS.

---

## ✨ Key Features

- 🏎️ **Massive Parallelism:** Multiplies 100,000+ object positions against the camera matrix in milliseconds on the GPU.
- 🧠 **Smart Precision Mode (Float16):** An optional toggle to compress 32-bit floats into 16-bit `System.Half`. This halves PCIe transfer times and VRAM usage for maximum performance.
- 🧮 **Clip Space Frustum Culling:** Uses the strict `-W` to `+W` homogeneous coordinates bounding box to perfectly cull objects off-screen.
- 🧵 **Multi-Threaded CPU Fallback:** Automatically utilizes `Parallel.For` to split the final Boolean logic across all available CPU cores.

---

## 📦 Installation

Install via the standard .NET CLI or NuGet Package Manager:

```bash
dotnet add package UltimateCameraCuller
```

## 🚀 Quick Start
Here is how easily you can integrate `UltimateCameraCuller` into your game engine's render loop:
```C#
using UltimateCameraCuller;
using System;

class GameLoop
{
    static void Main()
    {
        int numObjects = 100_000;
        
        // 1. Your Camera's View-Projection Matrix (4x4)
        float[,] cameraMatrix = new float[4, 4]; // Populated by your game engine

        // 2. Your 3D Objects' Positions (X, Y, Z, W)
        // W is usually 1.0f for physical positions
        float[,] objectPositions = new float[numObjects, 4];

        // ... Load your matrix and object data here ...

        // 3. Cull the invisible objects!
        // Setting useSmartPrecision = true will compress data to Float16 for extreme speed
        bool[] visibleObjects = GpuCameraCuller.GetVisibleObjects(cameraMatrix, objectPositions, useSmartPrecision: true);

        // 4. Render only what the camera sees
        for(int i = 0; i < numObjects; i++)
        {
            if (visibleObjects[i])
            {
                // DrawMesh(objects[i]);
            }
        }
        
        Console.WriteLine("Culling complete!");
    }
}
```
