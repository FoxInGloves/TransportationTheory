using System;
using TransportationTheory.Models.TransportationTheorySolver.Abstractions;

namespace TransportationTheory.Models.TransportationTheorySolver.Implementations;

public sealed class BasicTransportationTheorySolver : ITransportationTheorySolver
{
    public float[,] Calculate(in TransportationTheory.TransportationTheory matrix)
    {
        var consumers = new float[matrix.Consumers.Length];
        Array.Copy(matrix.Consumers, consumers, matrix.Consumers.Length);

        var supplies = new float[matrix.Supplies.Length];
        Array.Copy(matrix.Supplies, supplies, matrix.Supplies.Length);

        var result = new float[supplies.Length, consumers.Length];

        var solveMatrix = new float[supplies.Length, consumers.Length];

        int row = 0, column = 0;

        while (row < supplies.Length - 1 && column < consumers.Length - 1)
        {
            var product = MathF.Min(supplies[row], consumers[column]);

            solveMatrix[row, column] = product;

            supplies[row] -= product;

            consumers[column] -= product;

            if (supplies[row] == 0) row++;
            if (consumers[column] == 0) column++;
        }

        for (var i = 0; i < supplies.Length; i++)
        {
            for (var j = 0; j < consumers.Length; j++)
            {
                result[i, j] = solveMatrix[i, j];
            }

            result[i, consumers.Length - 1] = supplies[i];
        }

        for (var j = 0; j < consumers.Length; j++)
            result[supplies.Length - 1, j] = consumers[j];

        return TransformResult(result);
    }

    private float[,] TransformResult(float[,] input)
    {
        for (var i = 0; i < input.GetLength(0); i++)
        {
            for (var j = 0; j < input.GetLength(1); j++)
            {
                if (input[i, j] == 0)
                    input[i, j] = -1;
            }
        }

        return input;
    }
}