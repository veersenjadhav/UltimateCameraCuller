using System;
using System.Threading.Tasks;
using FastMatrixEngine;

namespace UltimateCameraCuller
{
    public static class GpuCameraCuller
    {
        /// <summary>
        /// Calculates which objects are visible. 
        /// Offloads heavy N^3 matrix multiplication to FastMatrixEngine (GPU), 
        /// then processes Frustum Culling on all CPU cores simultaneously.
        /// </summary>
        public static bool[] GetVisibleObjects(float[,] cameraViewProjMatrix, float[,] objectPositions, bool useSmartPrecision = false)
        {
            int numObjects = objectPositions.GetLength(0);
            int mCols = objectPositions.GetLength(1);

            if (mCols < 4)
                throw new ArgumentException("Matrices must have at least 4 columns to support 3D homogeneous coordinates (X, Y, Z, W).");

            bool[] visibleObjects = new bool[numObjects];

            if (useSmartPrecision)
            {
                // =======================================================
                // SMART PRECISION MODE (Float16)
                // =======================================================
                // 1. Compress 32-bit floats to 16-bit Half to save VRAM and PCIe bandwidth
                Half[,] halfPositions = SmartPrecisionHelper.ConvertToHalf(objectPositions);
                Half[,] halfMatrix = SmartPrecisionHelper.ConvertToHalf(cameraViewProjMatrix);

                // 2. Use your newly added FastMatrixEngine overload!
                Half[,] transformedPositions = GpuMatrixMultiplier.Multiply(halfPositions, halfMatrix);

                // 3. Parallel CPU Clip-Space Culling
                Parallel.For(0, numObjects, i =>
                {
                    // Cast Half back to float for the math comparison
                    float x = (float)transformedPositions[i, 0];
                    float y = (float)transformedPositions[i, 1];
                    float z = (float)transformedPositions[i, 2];
                    float w = (float)transformedPositions[i, 3];

                    // Clip Space Math: Inside the -W and +W bounds means it is on-screen
                    visibleObjects[i] = (x >= -w && x <= w) && 
                                        (y >= -w && y <= w) && 
                                        (z >= -w && z <= w);
                });
            }
            else
            {
                // =======================================================
                // STANDARD PRECISION MODE (Float32)
                // =======================================================
                // 1. Multiply directly using FastMatrixEngine
                float[,] transformedPositions = GpuMatrixMultiplier.Multiply(objectPositions, cameraViewProjMatrix);

                // 2. Parallel CPU Clip-Space Culling
                Parallel.For(0, numObjects, i =>
                {
                    float x = transformedPositions[i, 0];
                    float y = transformedPositions[i, 1];
                    float z = transformedPositions[i, 2];
                    float w = transformedPositions[i, 3];

                    visibleObjects[i] = (x >= -w && x <= w) && 
                                        (y >= -w && y <= w) && 
                                        (z >= -w && z <= w);
                });
            }

            return visibleObjects;
        }

        /// <summary>
        /// Double precision overload.
        /// Ignores SmartPrecision to prevent precision loss and 'Z-fighting' in game engines.
        /// </summary>
        public static bool[] GetVisibleObjects(double[,] cameraViewProjMatrix, double[,] objectPositions, bool useSmartPrecision = false)
        {
            int numObjects = objectPositions.GetLength(0);
            int mCols = objectPositions.GetLength(1);

            if (mCols < 4)
                throw new ArgumentException("Matrices must have at least 4 columns to support 3D homogeneous coordinates (X, Y, Z, W).");

            // 1. Offload Double multiplication to FastMatrixEngine
            double[,] transformedPositions = GpuMatrixMultiplier.Multiply(objectPositions, cameraViewProjMatrix);

            bool[] visibleObjects = new bool[numObjects];

            // 2. Parallel CPU Culling
            Parallel.For(0, numObjects, i =>
            {
                double x = transformedPositions[i, 0];
                double y = transformedPositions[i, 1];
                double z = transformedPositions[i, 2];
                double w = transformedPositions[i, 3];

                visibleObjects[i] = (x >= -w && x <= w) && 
                                    (y >= -w && y <= w) && 
                                    (z >= -w && z <= w);
            });

            return visibleObjects;
        }
    }
}