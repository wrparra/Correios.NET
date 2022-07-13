using Correios.NET;

RegisterServices.Register();
var _correiosService = RegisterServices.Get<ICorreiosService>();

var option = "0";
while (option != string.Empty)
{
    WriteLineWithColor("Correios.NET", ConsoleColor.Yellow);
    WriteLineWithColor("1 - Consultar CEP");
    WriteLineWithColor("2 - Consultar Encomenda");
    option = Console.ReadLine();
    if (LoadOption(option))
    {
        WriteLineWithColor("Pressione enter para continuar...");
        Console.ReadLine();
        Console.Clear();

    }
    else
    {
        WriteLineWithColor("App finalizado. Pressione enter para continuar...", ConsoleColor.Red);
        Console.ReadLine();
    }
}


bool IsValidOption(string option)
{
    string[] validOptions = { "1", "2" };
    return validOptions.Contains(option);
}

bool LoadOption(string option)
{
    if (string.IsNullOrEmpty(option))
        return false;

    if (!IsValidOption(option))
    {
        WriteLineWithColor("Opção inválida. Tente novamente.", ConsoleColor.Red);
    }

    switch (option)
    {
        case "1":
            LoadZipCode();
            break;
        case "2":
            LoadTracking();
            break;
        default:
            break;
    }

    return true;
}

void LoadTracking()
{
    Console.Clear();
    var code = string.Empty;
    while (string.IsNullOrEmpty(code))
    {
        WriteLineWithColor("Correios.NET", ConsoleColor.Yellow);
        WriteWithColor("Digite o Código de Rastreio de Encomendas: ");
        code = Console.ReadLine();
        if (!string.IsNullOrEmpty(code))
        {
            var tracking = _correiosService.GetPackageTracking(code);
            if (tracking != null)
            {
                if (tracking.TrackingHistory != null && tracking.TrackingHistory.Any())
                {
                    foreach (var history in tracking.TrackingHistory)
                    {
                        WriteLineWithColor($"{history}");
                    }
                }
            }
        }
    }
}

void LoadZipCode()
{
    Console.Clear();
    var zipCode = string.Empty;
    while (string.IsNullOrEmpty(zipCode))
    {
        WriteLineWithColor("Correios.NET", ConsoleColor.Yellow);
        WriteWithColor("Digite o CEP: ");
        zipCode = Console.ReadLine();
        if (!string.IsNullOrEmpty(zipCode))
        {
            var addresses = _correiosService.GetAddresses(zipCode);
            if (addresses != null && addresses.Any())
            {
                foreach (var address in addresses)
                {
                    WriteLineWithColor($"[{address.ZipCode}] {address.Street}, {address.District} - {address.City}/{address.State}");
                }
            }
        }
    }
}

void WriteLineWithColor(string value, ConsoleColor foreColor = ConsoleColor.White)
{
    var oldForeColor = Console.ForegroundColor;
    Console.ForegroundColor = foreColor;
    Console.WriteLine(value);
    Console.ForegroundColor = oldForeColor;
}

void WriteWithColor(string value, ConsoleColor foreColor = ConsoleColor.White)
{
    var oldForeColor = Console.ForegroundColor;
    Console.ForegroundColor = foreColor;
    Console.Write(value);
    Console.ForegroundColor = oldForeColor;
}