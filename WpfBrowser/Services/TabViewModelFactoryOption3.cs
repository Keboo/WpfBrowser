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

        //TODO: AutoDI resolves dependencies (IMessenger) with the global scope, at present there is not a way to leverage the scope created above..
        //Also because it is not instantiating the TabeViewModel, this will not be stored in the scope, so you will need to dispose of it itself.
        //It is also worth noting, that because you can directly instantiate the TabViewModel this way you really don't need the factory any more, this could be done directly in the MainWindowViewModel (this ignore the scope; for working with scopes like that I would still use a factory).
        //There is a little on the wiki explaining how this works:
        //https://github.com/Keboo/AutoDI/wiki/Quick-Start

        // If I understand correctly, AutoDI will merge resolved ctor parameters from DI container with the run-time data (tabName) to a complete parameter list by IL interweaving --- MAGIC :-)
        var tabViewModel = new TabViewModel(tabName);
        
        ServiceScopes[tabViewModel] = scope;
        return tabViewModel;
    }

    public void Receive(CloseTabRequest message)
    {
        if (ServiceScopes.TryRemove(message.TabViewModel, out var scope))
        {
            message.TabViewModel.Dispose();
            scope.Dispose();    // This will now dispose the TabViewModel because the DI container (scope) created it
        }
    }
}