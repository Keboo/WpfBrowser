using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using WpfBrowser.ViewModels;

namespace WpfBrowser.Services;

internal class TabViewModelFactoryOption3 : ITabViewModelFactory, IRecipient<CloseTabRequest>
{
    private ConcurrentDictionary<TabViewModel, IServiceScope> ServiceScopes { get; } = new();

    public TabViewModelFactoryOption3(IMessenger messenger)
    {
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
    ///  - Not sure that AutoDI actually supports scopes?
    /// </summary>
    public TabViewModel Create(string tabName)
    {
        // Most of this method is pseudo-code since I don't think AutoDI actually supports scopes?! For the sake of compilation, I have added simple classes to emulate the AutoDI API.
        var autoDiScope = GlobalDI.CreateScope();   // Seems like it is not supported?!
        
        var tabViewModel = autoDiScope.GetService<TabViewModel>(new object[] { tabName });    // If I understand correctly, AutoDI will merge resolved ctor parameters from DI container with the run-time data (tabName) to a complete parameter list by IL interweaving -- MAGIC :-)
        
        ServiceScopes[tabViewModel] = autoDiScope;
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

internal class GlobalDI
{
    public static GloablDIServiceScope CreateScope()
    {
        return default;
    }
}

internal class GloablDIServiceScope : IServiceScope
{
    public TServiceType GetService<TServiceType>(object[] parameters)
    {
        return default;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public IServiceProvider ServiceProvider { get; }
}