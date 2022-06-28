using Microsoft.Extensions.Hosting;
using System.Reflection;
using AutoDI;

namespace WpfBrowser;

internal static class AuotDIMixins
{
    public static IHostBuilder UseAutoDI(this IHostBuilder builder, Assembly? containerAssembly = null)
    {
        return builder.ConfigureServices(services => DI.AddServices(services, containerAssembly ?? Assembly.GetEntryAssembly()));
    }
}
