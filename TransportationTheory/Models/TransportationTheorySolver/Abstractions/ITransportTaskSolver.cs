namespace TransportationTheory.Models.TransportationTheorySolver.Abstractions;

public interface ITransportationTheorySolver
{
    float[,] Calculate(in TransportationTheory.TransportationTheory matrix);
}