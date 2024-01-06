using System;
using System.Linq;

namespace TransportTask.Models.TransportTask;

public sealed class TransportTaskFactory
{
    public TransportTask CreateInputMatrix(float[,] matrix)
    {
        float[,] tariffMatrix = CreateBaseTariffMatrix(matrix);

        var consumersMatrix = CreateConsumersMatrix(matrix);

        var suppliesMatrix = CreateSuppliesMatrix(matrix);

        var difference = suppliesMatrix.Sum() - consumersMatrix.Sum();

        if (difference > 0)
        {
            ModifyArray(ref consumersMatrix, difference);

            tariffMatrix = CreateTariffMatrixConsumers(matrix);
        }
        else
        if (difference < 0)
        {
            ModifyArray(ref suppliesMatrix, (uint)difference);

            tariffMatrix = CreateTariffMatrixSupplies(matrix);
        }

        return new TransportTask(tariffMatrix, consumersMatrix, suppliesMatrix);
    }

    private float[,] CreateTariffMatrixConsumers(float[,] matrix)
    {
        var tariffMatrix = new float[matrix.GetLength(0) - 1, matrix.GetLength(1)];

        for (var i = 0; i < tariffMatrix.GetLength(0); i++)
            tariffMatrix[i, tariffMatrix.GetLength(1) - 1] = 0;

        for (var i = 0; i < matrix.GetLength(0) - 1; i++)
        {
            for (var j = 0; j < matrix.GetLength(1) - 1; j++)
            {
                tariffMatrix[i, j] = matrix[i, j];
            }
        }

        return tariffMatrix;
    }

    private float[,] CreateTariffMatrixSupplies(float[,] matrix)
    {
        var tariffMatrix = new float[matrix.GetLength(0), matrix.GetLength(1) - 1];
        
        for (var i = 0; i < tariffMatrix.GetLength(1); i++)
            tariffMatrix[tariffMatrix.GetLength(0) - 1, i] = 0;
        
        for (var i = 0; i < matrix.GetLength(0) - 1; i++)
        {
            for (var j = 0; j < matrix.GetLength(1) - 1; j++)
            {
                tariffMatrix[i, j] = matrix[i, j];
            }
        }

        return tariffMatrix;
    }

    private float[,] CreateBaseTariffMatrix(float[,] matrix)
    {
        var tariffMatrix = new float[matrix.GetLength(0) - 1, matrix.GetLength(1) - 1];
        
        for (var i = 0; i < matrix.GetLength(0) - 1; i++)
        {
            for (var j = 0; j < matrix.GetLength(1) - 1; j++)
            {
                tariffMatrix[i, j] = matrix[i, j];
            }
        }

        return tariffMatrix;
    }

    private float[] CreateConsumersMatrix(float[,] matrix)
    {
        var consumersMatrix = new float[matrix.GetLength(1) - 1];

        for (var i = 0; i < matrix.GetLength(1) - 1; i++)
        {
            consumersMatrix[i] = matrix[matrix.GetLength(0) - 1, i]; //мб minus 2 надо, чтоб нули убрать
        }

        return consumersMatrix;
    }

    private float[] CreateSuppliesMatrix(float[,] matrix)
    {
        var suppliesMatrix = new float[matrix.GetLength(0) - 1];

        for (var i = 0; i < matrix.GetLength(0) - 1; i++)
        {
            suppliesMatrix[i] = matrix[i, matrix.GetLength(1) - 1]; //мб minus 2 надо, чтоб нули убрать
        }

        return suppliesMatrix;
    }

    private void ModifyArray(ref float[] array, float change)
    {
        Array.Resize(ref array, array.Length + 1);

        array[^1] = change;
    }
}