using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Toolkit.Mvvm.Messaging;
using WpfBrowser.Services;
using WpfBrowser.ViewModels;
using WpfBrowser.Views;

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
                    
                    //services.AddSingleton<ITabViewModelFactory, TabViewModelFactory>();                         // TabViewModels are not instantiated by DI container
                    //services.AddSingleton<ITabViewModelFactory, ContainerSupportedTabViewModelFactory>();       // TabViewModels are instantiated by DI container, but introduces "temporal coupling" and mutable Name property of the VM.
                    services.AddSingleton<ITabViewModelFactory, ScopedContainerSupportedTabViewModelFactory>();   // TabViewModels are instantiated by DI container in their own scope, but introduces "temporal coupling", mutable Name property of the VM, and issues with the lifetime handling of the scope.

                    services.AddTransient<TabViewModel>();
                    //services.AddScoped<TabViewModel>();   // This does not work as all tabs will share the same VM
                    services.AddScoped<ScopedTabViewModel>();
                });
        }
    }
}
