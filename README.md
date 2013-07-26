Correios.NET
================================
API .NET para consumo de serviços dos correios.

Como usar
-------------------------
Para instalar o Correios.NET, execute o seguinte comando no Package Manager Console.

	PM> Install-Package Correios.NET -pre


Rastreamento de encomendas/pacotes
-------------------------
Exemplo utilizando Console App com método sync

	class Program
    {
        static void Main(string[] args)
        {
            var result = new Correios.NET.Services().GetPackageTracking("SW000000000BR");

            foreach (var status in result.Statuses)
                Console.WriteLine("{0:dd/MM/yyyy HH:mm} - {1} - {2} - {3}", status.Date, status.Location, status.Situation, status.Details);

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

Roadmap
-------------------------
Próximas implementações

1. Cálculo de Frete
2. Busca CEP
3. Busca CEP por Logradouro
4. e outros... ( http://www.correios.com.br/webservices/ )
	
Contato
-------------------------

Caso tenha alguma dúvida ou sugestão entre em contato: wrparra (em) gmail.com

Copyright © 2013 Wellington R. Parra, released under the MIT license