using System;
using System.Diagnostics;
using MahApps.Metro.Controls;
using WpfBrowser.ViewModels;

namespace WpfBrowser.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MetroWindow
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

        InitializeComponent();
    }

    private void LaunchGitHubSite(object sender, System.Windows.RoutedEventArgs e)
    {
        Process.Start("explorer", "https://github.com/nicolaihenriksen/WpfBrowser");
    }
}