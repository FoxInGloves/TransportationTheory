using System.Windows;

namespace TransportationTheory.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        BasePlan.Visibility = Visibility.Visible;

        Optimize.Visibility = Visibility.Visible;
    }
}