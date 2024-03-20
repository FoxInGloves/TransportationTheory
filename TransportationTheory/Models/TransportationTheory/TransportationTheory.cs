namespace TransportationTheory.Models.TransportationTheory;

public readonly struct TransportationTheory
{
    public readonly float[,] TariffMatrix;

    public readonly float[] Consumers;

    public readonly float[] Supplies;

    public TransportationTheory(float[,] tariffMatrix, float[] consumers, float[] supplies)
    {
        TariffMatrix = tariffMatrix;
        Consumers = consumers;
        Supplies = supplies;
    }
}