using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary1;

public static class Class1
{
    public static void AddInternals(this IServiceCollection services)
    {
        services.AddSingleton<IService, MyInternalService>();
    }
}

internal class MyInternalService : IService
{
    
}

internal interface IService
{
}