using System.Collections.Generic;

namespace Correios.NET.Models
{
    public class CorreiosAddresResponse
    {
        public bool Erro { get; set; }
        public string Mensagem { get; set; }
        public int Total { get; set; }
        public IEnumerable<CorreiosAddress> Dados { get; set; }
    }
    public class CorreiosAddress
    {
        public string Uf { get; set; }
        public string Localidade { get; set; }
        public string LocalidadeSubordinada { get; set; }
        public string LogradouroDNEC { get; set; }
        public string LogradouroTextoAdicional { get; set; }
        public string LogradouroTexto { get; set; }
        public string Bairro { get; set; }
        public string NomeUnidade { get; set; }
        public string Cep { get; set; }
        public string TipoCep { get; set; }
        public string NumeroLocalidade { get; set; }
        public string Situacao { get; set; }
        public string[] FaixasCaixaPosta { get; set; }
        public string[] FaixasCep { get; set; }
    }
}