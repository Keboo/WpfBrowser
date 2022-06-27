using WpfBrowser.ViewModels;

namespace WpfBrowser.Services;

public interface ITabViewModelFactory
{
    TabViewModel Create(string tabName);
}