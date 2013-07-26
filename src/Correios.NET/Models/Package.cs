using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Correios.NET.Exceptions;
using Correios.NET.Extensions;
using CsQuery;

namespace Correios.NET.Models
{
    public class Package
    {
        public Package(string code)
        {
            Code = code;
            Statuses = new List<PackageStatus>();
        }

        public string Code { get; private set; }
        public IList<PackageStatus> Statuses { get; private set; }

        public PackageStatus CurrentStatus
        {
            get { return Statuses.FirstOrDefault(); }
        }

        public bool IsDelivered
        {
            get { return Statuses.Any() && CurrentStatus.Situation.Equals("Entrega Efetuada"); }
        }

        public void AddStatus(DateTime date, string location, string status, string details)
        {
            AddStatus(new PackageStatus
                      {
                          Date = date,
                          Location = location,
                          Situation = status,
                          Details = details
                      });
        }

        public void AddStatus(PackageStatus status)
        {
            Statuses.Add(status);
        }

        public override string ToString()
        {
            return Code;
        }

        /// <summary>
        /// Parse and converts the html page in a package
        /// </summary>
        /// <param name="html">html page</param>
        /// <returns>A Package</returns>
        /// <exception cref="Correios.NET.Exceptions.ParseException"></exception>
        public static Package Parse(string html)
        {
            CQ dom = html;
            var packageCode = ParsePackageCode(dom);
            var package = new Package(packageCode);
            PackageStatus status = null;

            var tableRows = dom.Select("table tr");
            if (tableRows.Length == 0)
                throw new ParseException(dom.Select("p").Text().RemoveLineEndings());

            try
            {
                foreach (var columns in tableRows.Skip(1).Select(tableRow => tableRow.ChildElements.ToList()))
                {
                    if (columns.Count == 3)
                    {
                        status = new PackageStatus();
                        if (columns[0].HasAttribute("rowspan"))
                        {
                            status.Date = DateTime.Parse(columns[0].InnerText);
                        }

                        status.Location = columns[1].InnerText;
                        status.Situation = columns[2].InnerText;

                        package.AddStatus(status);
                    }
                    else
                    {
                        if (status != null)
                            status.Details = columns[0].InnerText;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ParseException("Não foi possível converter o pacote/encomenda.", ex);
            }

            return package;
        }

        private static string ParsePackageCode(CQ dom)
        {
            var code = dom["input[name=P_ITEMCODE]"].Val();

            if (string.IsNullOrEmpty(code))
                throw new ParseException("Código da encomenda/pacote não foi encontrado.");

            return code;
        }
    }

    public class PackageStatus
    {
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Situation { get; set; }
        public string Details { get; set; }

        public override string ToString()
        {
            return string.Format("{0:dd/MM/yyyy HH:mm} - {1} - {2}", Date, Location, Situation);
        }

    }
}
