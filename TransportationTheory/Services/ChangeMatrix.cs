namespace TransportationTheory.Services;

public class ChangeMatrix
{
    public (string[,], float) ForOptimizeMatrix(float[,] matrix, float[,] tariff)
    {
        var newMatrix = new string[matrix.GetLength(0) - 1, matrix.GetLength(1) - 1];

        float minCost = 0f;

        for (var i = 0; i < matrix.GetLength(0) - 1; i++)
        {
            for (var j = 0; j < matrix.GetLength(1) - 1; j++)
            {
                newMatrix[i, j] = matrix[i, j].ToString();
                
                if(matrix[i,j] > -1)
                    minCost += matrix[i, j] * tariff[i, j];
            }
        }

        return (newMatrix, minCost);
    }
    
    public (string[,], float) ForBasicMatrix(float[,] matrix, float[,] tariff)
    {
        var newMatrix = new string[tariff.GetLength(0), tariff.GetLength(1)];

        var minCost = 0f;

        for (var i = 0; i < tariff.GetLength(0); i++)
        {
            for (var j = 0; j < tariff.GetLength(1); j++)
            {
                newMatrix[i, j] = matrix[i, j].ToString();
                
                if(matrix[i,j] > -1)
                    minCost += matrix[i, j] * tariff[i, j];
            }
        }

        return (newMatrix, minCost);
    }
    
    public string[,] RemoveUnnecessaryCells(string[,] matrix)
    {
        var newMatrix = new string[matrix.GetLength(0) + 1, matrix.GetLength(1) + 1];

        for (var i = 0; i < newMatrix.GetLength(1) - 1; i++)
            newMatrix[0, i + 1] = $"B{i}";

        for (var i = 0; i < newMatrix.GetLength(0) - 1; i++)
            newMatrix[i + 1, 0] = $"A{i}";

        for (var i = 0; i < matrix.GetLength(0); i++)
        {
            for (var j = 0; j < matrix.GetLength(1); j++)
            {
                newMatrix[i + 1, j + 1] = matrix[i, j].ToString();
            }
        }

        return newMatrix;
    }
}