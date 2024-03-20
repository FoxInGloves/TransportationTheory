using System;
using TransportationTheory.Models.TransportationTheorySolver.Abstractions;

namespace TransportationTheory.Models.TransportationTheorySolver.Implementations.Decorators;

public abstract class BaseTransportationTheorySolverDecorator : ITransportationTheorySolver
{
    protected readonly ITransportationTheorySolver TransportationTheorySolver;

    protected BaseTransportationTheorySolverDecorator(ITransportationTheorySolver transportationTheorySolver)
    {
        TransportationTheorySolver = transportationTheorySolver ?? throw new ArgumentNullException(nameof(transportationTheorySolver));
    }

    public abstract float[,] Calculate(in TransportationTheory.TransportationTheory matrix);
}