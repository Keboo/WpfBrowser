using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using WpfBrowser.ViewModels;

namespace WpfBrowser.Services;

internal class TabViewModelFactoryOption1 : ITabViewModelFactory, IRecipient<CloseTabRequest>
{
    private IServiceProvider ServiceProvider { get; }
    private IMessenger Messenger { get; }

    private ConcurrentDictionary<TabViewModel, IServiceScope> ServiceScopes { get; } = new();

    public TabViewModelFactoryOption1(IServiceProvider serviceProvider, IMessenger messenger)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        Messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        Messenger.Register(this);
    }

    /// <summary>
    /// New up the view model ourselves, and feed it the required services and run-time data
    ///
    /// PROS
    ///  - Does not break encapsulation (ie. TabViewModel.TabName can remain immutable)
    ///
    /// CONS
    ///  - Changes in ctor signature of the TabViewModel will require changes here as well
    ///  - Requires explicit disposal of the TabViewModel because the DI container (scope) did not create it
    /// </summary>
    public TabViewModel Create(string tabName)
    {
        var scope = ServiceProvider.CreateScope();
        //NB: Resolve any other DI dependencies that needed from the scope
        var tabViewModel = new TabViewModel(scope.ServiceProvider.GetRequiredService<IMessenger>(), tabName);
        ServiceScopes[tabViewModel] = scope;
        return tabViewModel;
    }

    public void Receive(CloseTabRequest message)
    {
        if (ServiceScopes.TryRemove(message.TabViewModel, out var scope))
        {
            message.TabViewModel.Dispose(); // Ugh!!! This is needed because we didn't let the DI container create the instance for us
            scope.Dispose();
        }
    }
}