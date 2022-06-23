using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using WpfBrowser.ViewModels;

namespace WpfBrowser.Services;

public interface ITabViewModelFactory
{
    TabViewModel Create(string tabName);
}

internal class TabViewModelFactory : ITabViewModelFactory
{
    protected readonly IServiceProvider ServiceProvider;

    public TabViewModelFactory(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public virtual TabViewModel Create(string tabName)
    {
        return new TabViewModel(ServiceProvider.GetRequiredService<IMessenger>(), tabName);     // Ugh!!! Needs to be updated if ctor signature changes
    }
}

internal class ContainerSupportedTabViewModelFactory : TabViewModelFactory
{
    public ContainerSupportedTabViewModelFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    { }

    public override TabViewModel Create(string tabName)
    {
        var viewModel = ServiceProvider.GetRequiredService<TabViewModel>();
        viewModel.TabName = tabName;    // !!! Code-smell !!! - Temporal coupling
        return viewModel;
    }
}

internal class ScopedContainerSupportedTabViewModelFactory : TabViewModelFactory
{
    public ScopedContainerSupportedTabViewModelFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    { }

    public override TabViewModel Create(string tabName)
    {
        var scope = ServiceProvider.CreateScope();  // How to dispose this scope when the tab is closed??
        var viewModel = scope.ServiceProvider.GetRequiredService<ScopedTabViewModel>();
        viewModel.TabName = tabName;    // !!! Code-smell !!! - Temporal coupling
        viewModel.Scope = scope;        // !!! Code-smell !!! - Temporal coupling
        return viewModel;
    }
}