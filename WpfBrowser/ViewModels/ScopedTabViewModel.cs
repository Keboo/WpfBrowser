using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace WpfBrowser.ViewModels;

internal class ScopedTabViewModel : TabViewModel
{
    public IServiceScope? Scope { get; set; }

    public ScopedTabViewModel(IMessenger? messenger) : base(messenger)
    { }

    protected override void CloseTab()
    {
        base.CloseTab();
        Scope?.Dispose();   // This does not feel like a good solution; a TabViewModel may be orphaned without calling CloseTab() for some reason and the scope lives for ever.
    }
}