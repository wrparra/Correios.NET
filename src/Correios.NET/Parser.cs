using Correios.NET.Exceptions;
using Correios.NET.Extensions;
using Correios.NET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Globalization;
using AngleSharp.Text;


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
        /// <exception cref="ParseException"></exception>
        public static IEnumerable<Address> ParseAddresses(string html)
        {
            //var config = Configuration.Default;
            //var context = BrowsingContext.New(config);
            //var document = context.OpenAsync(req => req.Content(html)).Result;            

            var document = new HtmlParser().ParseDocument(html);

            var content = document.QuerySelector("div.ctrlcontent");
            var responseText = content?.QuerySelector("p").Text();

            if (responseText == null || responseText.Equals("DADOS NAO ENCONTRADOS", StringComparison.InvariantCultureIgnoreCase))
                throw new ParseException("Endereço não encontrado.");

            var list = new List<Address>();

            var tableRows = content?.QuerySelectorAll("> table.tmptabela > tbody > tr").Skip(1);

            if (tableRows == null || !tableRows.Any())
                throw new ParseException("Endereço não encontrado.");

            foreach (var row in tableRows)
            {
                var address = row.Children;
                var street = address[0]?.Text().RemoveLineEndings();
                var district = address[1]?.Text().RemoveLineEndings();
                var cityState = address[2]?.Text().RemoveLineEndings().Split(new[] { '/' });

                if (cityState?.Length != 2)
                    throw new ParseException("Não foi possível extrair as informações de Cidade e Estado.");

                var city = cityState[0]?.Trim();
                var state = cityState[1]?.Trim();
                var zipcode = address[3]?.Text().RemoveHyphens();

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
        /// <exception cref="ParseException"></exception>
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
                var code = document.QuerySelector(".codSro")?.Text();

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

            PackageTracking status = null;
            var tableRows = document.QuerySelectorAll("table.listEvent.sro tbody tr");
            if (tableRows.Length == 0)
                throw new ParseException("Postagem não encontrada e/ou Aguardando postagem pelo remetente.");

            try
            {
                foreach (var columns in tableRows.Select(tr => tr.Children))
                {
                    if (columns.Count() == 2)
                    {
                        status = new PackageTracking();

                        var dateLocation = columns[0]?.Text().RemoveLineEndings();
                        var dateLocationSplitted = dateLocation?.SplitSpaces();
                        
                        if (dateLocationSplitted != null && dateLocationSplitted.Length >= 1)
                        {
                            status.Date = DateTime.Parse($"{dateLocationSplitted[0]} {dateLocationSplitted[1]}", CultureInfo.GetCultureInfo("pt-BR"));
                            status.Location = string.Join(" ", dateLocationSplitted.Skip(2)?.ToArray());
                            status.Status = columns[1].QuerySelector("strong")?.Text().RemoveLineEndings();
                        }

                        var descriptionSplitted = columns[1]?.Text().RemoveLineEndings().SplitSpaces(3);
                        if (descriptionSplitted != null && descriptionSplitted.Length > 1)
                            status.Details = string.Join(" ", descriptionSplitted.Skip(1).ToArray());

                        tracking.Add(status);
                    }
                    else
                    {
                        if (status != null)
                            status.Details = columns[0]?.Text().RemoveLineEndings();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ParseException("Não foi possível converter o pacote/encomenda.", ex);
            }

            if (tracking.Count == 0)
                throw new ParseException("Rastreamento não encontrado.");

            return tracking;
        }

        #endregion
    }
}
