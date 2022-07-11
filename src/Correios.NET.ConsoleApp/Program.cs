// See https://aka.ms/new-console-template for more information
WriteLineWithColor("Correios.NET", ConsoleColor.Yellow);
Console.WriteLine("1 - Consultar CEP");
Console.WriteLine("2 - Consultar Encomenda");
Console.ReadLine();


void WriteLineWithColor(string value, ConsoleColor foreColor = ConsoleColor.White)
{
    var oldForeColor = Console.ForegroundColor;
    Console.ForegroundColor = foreColor;
    Console.WriteLine(value);
    Console.ForegroundColor = oldForeColor;
}