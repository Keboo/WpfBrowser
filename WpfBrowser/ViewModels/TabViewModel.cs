using System;
using System.Diagnostics;
using System.Windows.Input;
using AutoDI;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace WpfBrowser.ViewModels;

public class TabViewModel : IDisposable
{
    public string TabName { get; internal set; }

    public string TabContentText => "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

    public ICommand CloseTabCommand { get; }

    private readonly IMessenger _messenger;

    /// <summary>
    /// Constructor used when instantiated by DI container
    /// </summary>
    public TabViewModel(IMessenger? messenger) : this(messenger, string.Empty)
    {
    }

    /// <summary>
    /// Constructor used when new'ed up directly in the factory
    /// </summary>
    public TabViewModel(IMessenger? messenger, string? tabName)
    {
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        TabName = tabName ?? throw new ArgumentNullException(nameof(tabName));

        CloseTabCommand = new RelayCommand(CloseTab);

        Debug.WriteLine($"[{GetHashCode()}] {GetType().Name} created");
    }

    public TabViewModel(string? tabName, [Dependency]IMessenger? messenger = null)
    {
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        TabName = tabName ?? throw new ArgumentNullException(nameof(tabName));

        CloseTabCommand = new RelayCommand(CloseTab);

        Debug.WriteLine($"[{GetHashCode()}] {GetType().Name} created");
    }

    protected virtual void CloseTab()
    {
        _messenger.Send(new CloseTabRequest(this));
    }

    public void Dispose()
    {
        // Dispose pattern may seem overkill in this simple scenario, but in the real-world setup which I am trying to exemplify with this simple demo application, I have components (eg. TabViewModel or similar) 
        // that create and maintain unmanaged resources, and thus I need a proper way of cleaning that up; Dispose pattern seems like a good match for that, even more so because the DI container (scope)
        // will dispose the "things" it create when it itself is disposed (at least all "things" that implement IDisposable)
        Debug.WriteLine($"[{GetHashCode()}] {GetType().Name} disposed");
    }
}