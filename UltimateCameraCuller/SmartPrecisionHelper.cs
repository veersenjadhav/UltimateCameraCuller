using System;

namespace UltimateCameraCuller
{
    /// <summary>
    /// Helper class to compress game engine positions into 16-bit Float16 arrays.
    /// This halves VRAM consumption and doubles PCIe transfer speeds.
    /// </summary>
    public static class SmartPrecisionHelper
    {
        public static Half[,] ConvertToHalf(float[,] input)
        {
            int rows = input.GetLength(0);
            int cols = input.GetLength(1);
            Half[,] result = new Half[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // Cast standard 32-bit float to 16-bit half
                    result[i, j] = (Half)input[i, j];
                }
            }
            return result;
        }
    }
}