using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using WpfBrowser.ViewModels;

namespace WpfBrowser.Services;

internal class TabViewModelFactoryOption2 : ITabViewModelFactory, IRecipient<CloseTabRequest>
{
    private IServiceProvider ServiceProvider { get; }

    private ConcurrentDictionary<TabViewModel, IServiceScope> ServiceScopes { get; } = new();

    public TabViewModelFactoryOption2(IServiceProvider serviceProvider, IMessenger messenger)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        messenger.Register(this);
    }

    /// <summary>
    /// Let the DI container create the VM for us and feed it the required run-time data afterwards
    ///
    /// PROS
    ///  - Changes in ctor signature of the TabViewModel will NOT require changes here
    ///  - Disposal of TabViewModels handled by scope implicitly (if it implements <see cref="IDisposable"/>)
    ///
    /// CONS
    ///  - Introduces temporal coupling (ie. TabViewModel.TabName "needs" to be set before the VM is in a valid state)
    ///  - Breaks encapsulation (ie. TabViewModel.TabName is no longer immutable)
    /// </summary>
    public TabViewModel Create(string tabName)
    {
        var scope = ServiceProvider.CreateScope();
        
        var tabViewModel = scope.ServiceProvider.GetRequiredService<TabViewModel>();    // This will call the ctor where the DI container can infer the required parameters (thus NOT the one including the run-time data!)
        
        tabViewModel.TabName = tabName; // Ugh!!! This introduces temporal coupling (ie. this MUST happen before the VM is used), and also breaks encapsulation (because TabViewModel.TabName is no longer immutable)

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