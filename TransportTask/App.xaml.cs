using System.Windows;
using TransportTask.Models.TransportTask;
using TransportTask.Models.TransportTaskSolver.Implementations;
using TransportTask.Models.TransportTaskSolver.Implementations.Decorators;

namespace TransportTask
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var basicTransportTaskSolver = new BasicTransportTaskSolver();
            
            var optimizedTransportTaskSolver = new TransportTaskSolverWithOptimizations(basicTransportTaskSolver);

            var inputMatrixFactory = new TransportTaskFactory();

            var viewModel = new ViewModels.MainViewModel(inputMatrixFactory, optimizedTransportTaskSolver);

            var view = new Views.MainWindow()
            {
                DataContext = viewModel
            };
            
            view.Show();
        }
    }
}