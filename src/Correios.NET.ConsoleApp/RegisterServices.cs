using Correios.NET;
using Microsoft.Extensions.DependencyInjection;

public static class RegisterServices
{
    private static IServiceCollection _services = new ServiceCollection();

    public static ServiceProvider ServiceProvider { get; set; }

    public static void Register()
    {
        _services.AddScoped<ICorreiosService, CorreiosService>();
        ServiceProvider = _services.BuildServiceProvider();
    }

    public static T Get<T>()
    {
        return ServiceProvider.GetService<T>();
    }
}
