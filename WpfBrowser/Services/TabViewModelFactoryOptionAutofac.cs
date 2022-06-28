using System;
using System.Collections.Concurrent;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using WpfBrowser.ViewModels;

namespace WpfBrowser.Services;

internal class TabViewModelFactoryOptionAutofac : ITabViewModelFactory, IRecipient<CloseTabRequest>
{
    private delegate TabViewModel ParameterizedTabViewModelFactory(string tabName);

    private IServiceProvider ServiceProvider { get; }

    private ConcurrentDictionary<TabViewModel, ILifetimeScope> ServiceScopes { get; } = new();

    public TabViewModelFactoryOptionAutofac(IServiceProvider serviceProvider, IMessenger messenger)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        messenger.Register(this);
    }

    /// <summary>
    /// Let the Autofac container create the VM for us and feed it the required run-time data using "parameterized instantiation" (see ParameterizedTabViewModelFactory delegate)
    ///
    /// PROS
    ///  - Changes in ctor signature of the TabViewModel will NOT require changes here (unless it is the dynamic data in which case the ParameterizedTabViewModelFactory needs to reflect that)
    ///  - Disposal of TabViewModels handled by scope implicitly (if it implements <see cref="IDisposable"/>)
    ///
    /// CONS
    ///  - None that I know of (at the time of writing)
    /// </summary>
    public TabViewModel Create(string tabName)
    {
        var scope = ServiceProvider.GetAutofacRoot().BeginLifetimeScope();      // Considerations here about which scope to start a new lifetime scope from needs to be considered. For my case, the root is the correct scope to start from.
        var factory = scope.Resolve<ParameterizedTabViewModelFactory>();

        var tabViewModel = factory.Invoke(tabName);

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