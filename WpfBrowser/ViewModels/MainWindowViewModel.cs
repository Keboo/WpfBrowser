using System;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using WpfBrowser.Services;

namespace WpfBrowser.ViewModels;

public class MainWindowViewModel : ObservableObject, IRecipient<CloseTabRequest>
{
    public IRelayCommand CreateNewTabCommand { get; }

    public ObservableCollection<TabViewModel> Tabs { get; } = new();

    public TabViewModel? SelectedTab
    {
        get => _selectedTab;
        set => SetProperty(ref _selectedTab, value);
    }

    private readonly IMessenger _messenger;
    private readonly ITabViewModelFactory _tabViewModelFactory;
    public MyControlViewModel ControlViewModel { get; }
    private int _tabCount;
    private TabViewModel? _selectedTab;

    public MainWindowViewModel(IMessenger messenger,
        ITabViewModelFactory tabViewModelFactory,
        MyControlViewModel controlViewModel)
    {
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _tabViewModelFactory = tabViewModelFactory ?? throw new ArgumentNullException(nameof(tabViewModelFactory));
        ControlViewModel = controlViewModel ?? throw new ArgumentNullException(nameof(controlViewModel));
        CreateNewTabCommand = new RelayCommand(CreateNewTab, () => Tabs.Count < 5);
        
        _messenger.Register(this);
    }

    private void CreateNewTab()
    {
        _tabCount++;
        var tabName = $"New tab {_tabCount}";
        var vm = _tabViewModelFactory.Create(tabName);
        Tabs.Add(vm);
        CreateNewTabCommand.NotifyCanExecuteChanged();
        SelectedTab = vm;
    }

    public void Receive(CloseTabRequest message)
    {
        if (Tabs.Remove(message.TabViewModel))
        {
            CreateNewTabCommand.NotifyCanExecuteChanged();
        }
    }
}

public record CloseTabRequest(TabViewModel TabViewModel);