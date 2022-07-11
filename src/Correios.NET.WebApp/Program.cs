using Correios.NET;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ICorreiosService, CorreiosService>();

var app = builder.Build();
app.UseHttpsRedirection();

app.MapGet("/cep/{zipCode}", async (string zipCode, ICorreiosService correiosService) =>
{
    return await correiosService.GetAddressesAsync(zipCode);
});

app.MapGet("/encomendas/{code}", async (string code, ICorreiosService correiosService) =>
{
    return await correiosService.GetPackageTrackingAsync(code);
});

app.Run();