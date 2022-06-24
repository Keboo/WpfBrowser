using System;
using System.Windows.Controls;
using WpfBrowser.ViewModels;

namespace WpfBrowser.Views;

/// <summary>
/// Interaction logic for MyUserControl.xaml
/// </summary>
public partial class MyUserControl : UserControl
{
    public MyUserControl()
    {
        InitializeComponent();
    }
    

    public MyUserControl(MyControlViewModel viewModel)
    {
        DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        InitializeComponent();
    }
}
