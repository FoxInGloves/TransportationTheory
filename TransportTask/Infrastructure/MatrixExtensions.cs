using System;

namespace TransportTask.Infrastructure;

public static class MatrixExtensions0
{
    public static T[,] Resize2d<T>(this T[,] original, int x, int y)
    {
        var newArray = new T[x, y];

        var minRows = Math.Min(x, original.GetLength(0));

        var minCols = Math.Min(y, original.GetLength(1));

        for (var i = 0; i < minRows; i++)
        {
            for (var j = 0; j < minCols; j++)
            {
                newArray[i, j] = original[i, j];
            }
        }

        return newArray;
    }
}