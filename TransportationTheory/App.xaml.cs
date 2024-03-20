using System.Windows;
using TransportationTheory.Models.TransportationTheory;
using TransportationTheory.Models.TransportationTheorySolver.Implementations;
using TransportationTheory.Models.TransportTaskSolver.Implementations;
using TransportationTheory.Models.TransportTaskSolver.Implementations.Decorators;

namespace TransportationTheory
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var basicTransportTaskSolver = new BasicTransportationTheorySolver();
            
            var optimizedTransportTaskSolver = new TransportationTheorySolverWithOptimizations(basicTransportTaskSolver);

            var inputMatrixFactory = new TransportationTheoryFactory();

            var viewModel = new ViewModels.MainViewModel(inputMatrixFactory, optimizedTransportTaskSolver);

            var view = new Views.MainWindow()
            {
                DataContext = viewModel
            };
            
            view.Show();
        }
    }
}