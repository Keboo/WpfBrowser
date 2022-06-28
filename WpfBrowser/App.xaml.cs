using System;
using System.Windows;
using Autofac;
using Autofac.Extensions.DependencyInjection;
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
        private const bool UseAutofac = true;

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
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    //TODO: Any config setup here
                });

            if (UseAutofac)
            {
                builder.UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureContainer<ContainerBuilder>(services =>
                    {
                        services.RegisterType<WeakReferenceMessenger>().As<IMessenger>().SingleInstance();

                        services.RegisterType<MainWindow>().SingleInstance();
                        services.RegisterType<MainWindowViewModel>();
                        services.RegisterType<TabViewModelFactoryOptionAutofac>().As<ITabViewModelFactory>().SingleInstance();
                        services.RegisterType<TabViewModel>().InstancePerLifetimeScope();
                    });
            }
            else
            {
                builder.ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IMessenger, WeakReferenceMessenger>();

                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<ITabViewModelFactory, TabViewModelFactoryOption1>();
                    services.AddScoped<TabViewModel>();
                });
            }

            return builder;
        }
    }
}
