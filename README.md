![Correios.NET](./src/Correios.NET/icon.png)

# Correios.NET

[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/wrparra/Correios.NET/.NET%20Core?logo=github&style=flat-square)](https://github.com/wrparra/Correios.NET/actions)
[![AppVeyor Build Status](https://img.shields.io/appveyor/build/wrparra/correios-net?logo=appveyor&style=flat-square)](https://ci.appveyor.com/project/wrparra/correios-net)
[![GitHub release](https://img.shields.io/github/v/release/wrparra/Correios.NET?logo=github&style=flat-square)](https://github.com/wrparra/Correios.NET/releases)
[![GitHub Tag](https://img.shields.io/github/tag/wrparra/Correios.NET.svg?logo=github&style=flat-square)](https://github.com/wrparra/Correios.NET/tags)
[![NuGet Count](https://img.shields.io/nuget/dt/Correios.NET.svg?logo=nuget&style=flat-square)](https://www.nuget.org/packages/Correios.NET/)
[![Issues Open](https://img.shields.io/github/issues/wrparra/Correios.NET.svg?logo=github&style=flat-square)](https://github.com/wrparra/Correios.NET/issues)

Biblioteca .NET para integração com os serviços dos Correios.
Serviços disponíveis até o momento.
- Consulta de CEPs
- Consulta de Encomendas

## Como usar

Para instalar o Correios .NET por linha de comando:

Usando Nuget no Package Manager Console

	PM> Install-Package Correios.NET
    
Usando CLI do .NET Core

    > dotnet add package Correios.NET

## Rastreamento de encomendas/pacotes

Exemplo utilizando Console App com método sync

	class Program
    {
        static void Main(string[] args)
        {
            var result = new Correios.NET.Services().GetPackageTracking("SW000000000BR");

            foreach (var track in result.TrackingHistory)
                Console.WriteLine("{0:dd/MM/yyyy HH:mm} - {1} - {2} - {3}", track.Date, track.Location, track.Status, track.Details);

            Console.ReadLine();
        }
    }
	
Exemplo utilizando ASP.NET MVC com método async

	public class HomeController : AsyncController
    {
        public async Task<ActionResult> Index()
        {
            var package = new Correios.NET.Services().GetPackageTrackingAsync("SW000000000BR");
            await Task.WhenAll(package);
            ViewBag.TrackingCode = package.Result.Code;
            return View();
        }
    }

## Consulta de Endereços por CEP

A consulta é feita informando um CEP com 8 dígitos, e neste caso somente um endereço é retornado.
Ao informar um CEP com menos de 8 dígitos a busca é feita retornando todos os CEPs de acordo com o intervalo informado.

Exemplo utilizando Console App com método sync

	class Program
    {
        static void Main(string[] args)
        {
            var addresses = new Correios.NET.Services().GetAddresses("15000000");

            foreach(var address in addresses)
                Console.WriteLine("{0} - {1} - {2} - {3}/{4}", address.ZipCode, address.Street, address.District, address.City, address.State);
            
            Console.ReadLine();
        }
    }
	
Exemplo utilizando ASP.NET MVC com método async

	public class HomeController : AsyncController
    {
        public async Task<ActionResult> Index()
        {
            var addresses = await new Correios.NET.Services().GetAddressesAsync("15000000");            
            return View();
        }
    }

	
## Roadmap

Próximas implementações

1. Cálculo de Frete
2. Busca CEP por Logradouro
3. e outros...
	
## Contato

Caso tenha alguma dúvida ou sugestão entre em contato: wrparra (em) gmail.com

Copyright © 2013-2020 Wellington R. Parra, released under the MIT license