using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Correios.NET.Exceptions;
using Correios.NET.Extensions;
using Correios.NET.Models;
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
            //var config = Configuration.Default;
            //var context = BrowsingContext.New(config);
            //var document = context.OpenAsync(req => req.Content(html)).Result;            

            var document = new HtmlParser().ParseDocument(html);

            var content = document.QuerySelector("div.ctrlcontent");
            var responseText = content.QuerySelector("p").Text();

            if (responseText == "DADOS NAO ENCONTRADOS")
                throw new ParseException("Endereço não encontrado.");

            var list = new List<Address>();

            var tableRows = content.QuerySelectorAll("> table.tmptabela > tbody > tr").Skip(1);

            if (tableRows.Count() == 0)
                throw new ParseException("Endereço não encontrado.");

            foreach (var row in tableRows)
            {
                var address = row.Children;
                var street = address[0].Text().RemoveLineEndings();
                var district = address[1].Text().RemoveLineEndings();
                var cityState = address[2].Text().RemoveLineEndings().Split(new[] { '/' });

                if (cityState.Length != 2)
                    throw new ParseException("Não foi possível extrair as informações de Cidade e Estado.");

                var city = cityState[0].Trim();
                var state = cityState[1].Trim();
                var zipcode = address[3].Text().RemoveHyphens();

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
            try
            {
                var document = new HtmlParser().ParseDocument(html);
                var packageCode = ParsePackageCode(document);
                var package = new Package(packageCode);
                package.AddTrackingInfo(ParsePackageTracking(document));
                return package;
            }
            catch (ParseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ParseException("Não foi possível converter o pacote/encomenda.", ex);
            }
        }

        private static string ParsePackageCode(IHtmlDocument document)
        {
            try
            {
                var code = document.QuerySelector("#page > main > .sub_header_in > .container > h1").Text().Replace("Rastreamento de Objeto - ", string.Empty);

                if (string.IsNullOrEmpty(code))
                    throw new ParseException("Código da encomenda/pacote não foi encontrado.");

                return code;
            }
            catch (ParseException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                throw new ParseException("Código da encomenda/pacote não foi encontrado.");
            }
        }

        private static IEnumerable<PackageTracking> ParsePackageTracking(IHtmlDocument document)
        {
            var tracking = new List<PackageTracking>();

            PackageTracking trackingStatus = null;
            var statusLines = document.QuerySelectorAll(".singlepost > ul.linha_status");
            if (statusLines.Length == 0)
                throw new ParseException("Postagem não encontrada e/ou Aguardando postagem pelo remetente.");

            const string packageDateTimePattern = @"[\w\s\:]*(\d{2}\/\d{2}\/\d{4})[\w\|\s\:]*(\d{2}\:\d{2})";

            try
            {
                foreach (var lines in statusLines.Select(ul => ul.Children))
                {
                    trackingStatus = new PackageTracking();
                    trackingStatus.Status = lines[0].QuerySelector("b").Text().RemoveLineEndings();
                    trackingStatus.Date = lines[1].Text().ExtractDateTime(packageDateTimePattern);
                    trackingStatus.Source = lines[2].Text().RemoveLineEndings().Replace("Origem: ", string.Empty).Replace("Local: ", string.Empty);

                    if (lines.Length >= 4)
                        trackingStatus.Destination = lines[3].Text().RemoveLineEndings().Replace("Destino: ", string.Empty);

                    tracking.Add(trackingStatus);
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
