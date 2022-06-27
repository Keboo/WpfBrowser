using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using WpfBrowser.ViewModels;

namespace WpfBrowser.Services;

internal class TabViewModelFactoryOption3 : ITabViewModelFactory, IRecipient<CloseTabRequest>
{
    private IServiceProvider ServiceProvider { get; }

    private ConcurrentDictionary<TabViewModel, IServiceScope> ServiceScopes { get; } = new();

    public TabViewModelFactoryOption3(IServiceProvider serviceProvider, IMessenger messenger)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        messenger.Register(this);
    }

    /// <summary>
    /// Leverage AutoDI to create the VM for us and IL interweave the run-time data to preserve encapsulation
    ///
    /// PROS
    ///  - Does not break encapsulation (ie. TabViewModel.TabName can remain immutable)
    ///  - Changes in ctor signature of the TabViewModel will NOT require changes here
    ///  - Disposal of TabViewModels handled by scope implicitly (if it implements <see cref="IDisposable"/>)
    ///
    /// CONS
    ///  - Breaks live unit testing
    ///  - Other drawbacks?
    /// </summary>
    public TabViewModel Create(string tabName)
    {
        // For the sake of compilation, I have added a "dummy" ServiceProviderMixins just to show the AutoDI GetService() overload that takes parameters (in order to emulate the AutoDI API).
        var scope = ServiceProvider.CreateScope();

        // If I understand correctly, AutoDI will merge resolved ctor parameters from DI container with the run-time data (tabName) to a complete parameter list by IL interweaving --- MAGIC :-)
        var tabViewModel = scope.ServiceProvider.GetService<TabViewModel>(new object[] { tabName });
        
        ServiceScopes[tabViewModel] = scope;
        return tabViewModel;
    }

    public void Receive(CloseTabRequest message)
    {
        if (ServiceScopes.TryRemove(message.TabViewModel, out var scope))
        {
            scope.Dispose();    // This will now dispose the TabViewModel because the DI container (scope) created it
        }
    }
}

public static class ServiceProviderMixins
{
    public static T GetService<T>(this IServiceProvider provider, params object[] autoDiParameters)
    {
        return default;
    }
}