using Correios.NET.Exceptions;
using Correios.NET.Extensions;
using Correios.NET.Models;
using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Correios.NET
{
    public class Parser
    {
        #region Address

        /// <summary>
        /// Parse and converts the html page in a zip address
        /// </summary>
        /// <param name="html">html page</param>
        /// <returns>An Address</returns>
        /// <exception cref="Correios.NET.Exceptions.ParseException"></exception>
        public static IEnumerable<Address> ParseAddresses(string html)
        {
            CQ dom = html;
            var main = dom["div.ctrlcontent"];
            var responseText = main.Find("> p:first").Text();

            if (responseText == "DADOS NAO ENCONTRADOS")
                throw new ParseException("Endereço não encontrado.");

            var list = new List<Address>();

            var tableRows = main.Find("> table.tmptabela > tbody > tr:not(:first)");

            if (tableRows.Length == 0)
                throw new ParseException("Endereço não encontrado.");

            foreach (var row in tableRows)
            {
                var address = row.ChildElements.ToList();
                var street = address[0].InnerText.RemoveLineEndings();
                var district = address[1].InnerText.RemoveLineEndings();
                var cityState = address[2].InnerText.RemoveLineEndings().Split(new[] { '/' });

                if (cityState.Length != 2)
                    throw new ParseException("Não foi possível extrair as informações de Cidade e Estado.");

                var city = cityState[0].Trim();
                var state = cityState[1].Trim();
                var zipcode = address[3].InnerText.RemoveHyphens();

                list.Add(new Address
                {
                    Street = street,
                    ZipCode = zipcode,
                    District = district,
                    City = city,
                    State = state
                });
            }


            return list;
        }

        #endregion

        #region Package

        /// <summary>
        /// Parse and converts the html page in a package
        /// </summary>
        /// <param name="html">html page</param>
        /// <returns>A Package</returns>
        /// <exception cref="Correios.NET.Exceptions.ParseException"></exception>
        public static Package ParsePackage(string html)
        {
            CQ dom = html;
            var packageCode = ParsePackageCode(dom);
            var package = new Package(packageCode);
            package.AddTrackingInfo(ParsePackageTracking(dom));
            return package;
        }

        private static string ParsePackageCode(CQ dom)
        {
            var code = string.Empty;
            var resultTitle = dom["body > p:first"].Text();

            if (!string.IsNullOrEmpty(resultTitle) && resultTitle.Contains("-"))
                code = resultTitle.Split('-')[0].Trim();

            if (string.IsNullOrEmpty(code))
                throw new ParseException("Código da encomenda/pacote não foi encontrado.");

            return code;
        }

        private static IEnumerable<PackageTracking> ParsePackageTracking(CQ dom)
        {
            var tracking = new List<PackageTracking>();

            PackageTracking status = null;
            var tableRows = dom.Select("table tr");
            if (tableRows.Length == 0)
                throw new ParseException(dom.Select("p").Text().RemoveLineEndings());

            try
            {
                foreach (var columns in tableRows.Skip(1).Select(tableRow => tableRow.ChildElements.ToList()))
                {
                    if (columns.Count == 3)
                    {
                        status = new PackageTracking();
                        if (columns[0].HasAttribute("rowspan"))
                        {
                            status.Date = DateTime.Parse(columns[0].InnerText.RemoveLineEndings());
                        }

                        status.Location = columns[1].InnerText.RemoveLineEndings();
                        status.Status = columns[2].InnerText.RemoveLineEndings();

                        tracking.Add(status);
                    }
                    else
                    {
                        if (status != null)
                            status.Details = columns[0].InnerText.RemoveLineEndings();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ParseException("Não foi possível converter o pacote/encomenda.", ex);
            }

            if (tracking.Count() == 0)
                throw new ParseException("Rastreamento não encontrado.");

            return tracking;
        }

        #endregion
    }
}
