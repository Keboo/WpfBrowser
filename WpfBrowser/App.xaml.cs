using System;
using System.Windows;
using AutoDI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Toolkit.Mvvm.Messaging;
using WpfBrowser.Services;
using WpfBrowser.ViewModels;
using WpfBrowser.Views;

[assembly:AutoDI.Settings(
    DebugLogLevel = AutoDI.DebugLogLevel.Verbose,
    DebugCodeGeneration = AutoDI.CodeLanguage.CSharp
)]

namespace WpfBrowser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            host.Start();
            
            GlobalDI.Register(host.Services);

            var app = new App();
            app.InitializeComponent();
            app.MainWindow = host.Services.GetRequiredService<MainWindow>();
            app.MainWindow.Visibility = Visibility.Visible;
            app.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    //TODO: Any config setup here
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IMessenger, WeakReferenceMessenger>();

                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<ITabViewModelFactory, TabViewModelFactoryOption3>();
                    services.AddScoped<TabViewModel>();
                })
                .UseAutoDI()
                ;
        }
    }
}
