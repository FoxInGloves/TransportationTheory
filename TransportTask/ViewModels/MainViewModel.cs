using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TransportTask.Infrastructure;
using TransportTask.Models.TransportTask;
using TransportTask.Models.TransportTaskSolver.Abstractions;
using TransportTask.Models.TransportTaskSolver.Implementations;
using TransportTask.Services;

namespace TransportTask.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly ITransportTaskSolver _transportTaskSolver;

    private readonly TransportTaskFactory _transportTaskFactory;

    private readonly ChangeMatrix _changeMatrix;

    [ObservableProperty] 
    private float[,] _matrix;
    
    [ObservableProperty] 
    private string[,] _basicMatrix;

    [ObservableProperty] 
    private string[,] _optimizeMatrix;
    
    [ObservableProperty] 
    private string _minCostBasicMatrix;
    
    [ObservableProperty] 
    private string _minCostOptimizeMatrix;

    public MainViewModel(TransportTaskFactory matrixFactory, ITransportTaskSolver transportTaskSolver)
    {
        _transportTaskFactory = matrixFactory ?? throw new ArgumentNullException(nameof(matrixFactory));
        
        _transportTaskSolver = transportTaskSolver ?? throw new ArgumentNullException(nameof(transportTaskSolver));

        _changeMatrix = new ChangeMatrix();
        
        Matrix = new [,]
        {
            { 1.1f, 2.6f, 1.9f, 2.2f, 7250 },
            { 0.8f, 2.4f, 2f, 2.1f, 10150 },
            { 1.6f, 3.4f, 2.8f, 1.7f, 4350 },
            { 8800, 5800, 2900, 2100, 0 }
        };

        AddRowCommand =
            new RelayCommand(() => { Matrix = Matrix.Resize2d(Matrix.GetLength(0) + 1, Matrix.GetLength(1)); },
                () => true);

        RemoveRowCommand =
            new RelayCommand(() => { Matrix = Matrix.Resize2d(Matrix.GetLength(0) - 1, Matrix.GetLength(1)); },
                () => Matrix.GetLength(0) - 1 >= 0);

        AddColumnCommand =
            new RelayCommand(() => { Matrix = Matrix.Resize2d(Matrix.GetLength(0), Matrix.GetLength(1) + 1); },
                () => true);

        RemoveColumnCommand =
            new RelayCommand(() => { Matrix = Matrix.Resize2d(Matrix.GetLength(0), Matrix.GetLength(1) - 1); },
                () => Matrix.GetLength(1) - 1 >= 0);
    }

    public ICommand AddRowCommand { get; private set; }

    public ICommand RemoveRowCommand { get; private set; }

    public ICommand AddColumnCommand { get; private set; }

    public ICommand RemoveColumnCommand { get; private set; }

    [RelayCommand]
    private void GetBasePlan()
    {
        var inputMatrix = _transportTaskFactory.CreateInputMatrix(Matrix);

        var forBasicMatrix = _changeMatrix.ForBasicMatrix(new BasicTransportTaskSolver().Calculate(inputMatrix), inputMatrix.TariffMatrix);

        BasicMatrix = _changeMatrix.RemoveUnnecessaryCells(forBasicMatrix.Item1);

        MinCostBasicMatrix = ChangeTextForOutput(forBasicMatrix.Item2);

        var forOptimizeMatrix = _changeMatrix.ForOptimizeMatrix(_transportTaskSolver.Calculate(inputMatrix), inputMatrix.TariffMatrix);

        OptimizeMatrix = _changeMatrix.RemoveUnnecessaryCells(forOptimizeMatrix.Item1);

        MinCostOptimizeMatrix = ChangeTextForOutput(forOptimizeMatrix.Item2);
    }

    private string ChangeTextForOutput(float inputText)
    {
        return $"Стоимость перевозок = {inputText}";
    }
}