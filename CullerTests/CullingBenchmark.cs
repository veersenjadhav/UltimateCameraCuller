using System;
using BenchmarkDotNet.Attributes;
using UltimateCameraCuller;

namespace MathTests
{
    // Tracks memory allocations, garbage collection, and ranks the fastest methods.
    [MemoryDiagnoser]
    [RankColumn]
    public class CullingBenchmark
    {
        // Testing with 100,000 objects (like a dense forest or large city game level)
        private const int NumObjects = 100_000;
        private const int MatrixCols = 4; // X, Y, Z, W for homogeneous 3D coordinates

        private float[,] _cameraMatrix;
        private float[,] _objectPositions;

        [GlobalSetup]
        public void Setup()
        {
            _cameraMatrix = new float[MatrixCols, MatrixCols];
            _objectPositions = new float[NumObjects, MatrixCols];

            var rand = new Random(42); // Fixed seed so tests are identical every run

            // 1. Generate a mock 4x4 View-Projection Matrix
            for (int i = 0; i < MatrixCols; i++)
            {
                for (int j = 0; j < MatrixCols; j++)
                {
                    _cameraMatrix[i, j] = (float)rand.NextDouble();
                }
            }

            // 2. Generate 100,000 random 3D objects
            for (int i = 0; i < NumObjects; i++)
            {
                // X, Y, Z positions
                _objectPositions[i, 0] = (float)(rand.NextDouble() * 1000 - 500); 
                _objectPositions[i, 1] = (float)(rand.NextDouble() * 1000 - 500); 
                _objectPositions[i, 2] = (float)(rand.NextDouble() * 1000 - 500); 
                
                // W is almost always 1.0 for physical object positions in game engines
                _objectPositions[i, 3] = 1.0f; 
            }
        }

        /// <summary>
        /// The Baseline: A standard single-threaded C# for-loop doing the exact same math.
        /// BenchmarkDotNet sets this as the 1.00x ratio mark.
        /// </summary>
        [Benchmark(Baseline = true)]
        public bool[] StandardCpuCulling()
        {
            bool[] visibleObjects = new bool[NumObjects];

            for (int i = 0; i < NumObjects; i++)
            {
                float x = 0f, y = 0f, z = 0f, w = 0f;

                // Matrix Multiplication for one object
                for (int j = 0; j < MatrixCols; j++)
                {
                    float pos = _objectPositions[i, j];
                    x += pos * _cameraMatrix[j, 0];
                    y += pos * _cameraMatrix[j, 1];
                    z += pos * _cameraMatrix[j, 2];
                    w += pos * _cameraMatrix[j, 3];
                }

                // Clip Space Frustum Check
                visibleObjects[i] = (x >= -w && x <= w) && 
                                    (y >= -w && y <= w) && 
                                    (z >= -w && z <= w);
            }

            return visibleObjects;
        }

        /// <summary>
        /// Your new package using standard 32-bit float precision.
        /// </summary>
        [Benchmark]
        public bool[] GpuCulling_StandardFloat()
        {
            return GpuCameraCuller.GetVisibleObjects(_cameraMatrix, _objectPositions, useSmartPrecision: false);
        }

        /// <summary>
        /// Your new package using the 16-bit Half precision optimization.
        /// </summary>
        [Benchmark]
        public bool[] GpuCulling_SmartPrecision()
        {
            return GpuCameraCuller.GetVisibleObjects(_cameraMatrix, _objectPositions, useSmartPrecision: true);
        }
    }
}