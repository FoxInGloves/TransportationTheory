namespace TransportTask.Models.TransportTaskSolver.Abstractions;

public interface ITransportTaskSolver
{
    float[,] Calculate(in TransportTask.TransportTask matrix);
}