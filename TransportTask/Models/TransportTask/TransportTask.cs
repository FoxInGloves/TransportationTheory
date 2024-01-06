namespace TransportTask.Models.TransportTask;

public readonly struct TransportTask
{
    public readonly float[,] TariffMatrix;

    public readonly float[] Consumers;

    public readonly float[] Supplies;

    public TransportTask(float[,] tariffMatrix, float[] consumers, float[] supplies)
    {
        TariffMatrix = tariffMatrix;
        Consumers = consumers;
        Supplies = supplies;
    }
}