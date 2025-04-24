using System.Windows;
using TransportationTheory.Models.TransportationTheorySolver.Implementations;

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

            var viewModel = new ViewModels.MainViewModel(optimizedTransportTaskSolver);

            var view = new Views.MainWindow()
            {
                DataContext = viewModel
            };
            
            view.Show();
        }
    }
}