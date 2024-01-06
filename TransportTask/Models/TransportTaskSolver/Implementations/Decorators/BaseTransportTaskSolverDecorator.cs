using System;
using TransportTask.Models.TransportTaskSolver.Abstractions;

namespace TransportTask.Models.TransportTaskSolver.Implementations.Decorators;

public abstract class BaseTransportTaskSolverDecorator : ITransportTaskSolver
{
    protected readonly ITransportTaskSolver TransportTaskSolver;

    protected BaseTransportTaskSolverDecorator(ITransportTaskSolver transportTaskSolver)
    {
        TransportTaskSolver = transportTaskSolver ?? throw new ArgumentNullException(nameof(transportTaskSolver));
    }

    public abstract float[,] Calculate(in TransportTask.TransportTask matrix);
}