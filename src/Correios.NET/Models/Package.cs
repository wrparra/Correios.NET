using Correios.NET.Exceptions;
using Correios.NET.Extensions;
using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Correios.NET.Models
{
    public class Package
    {
        private const string DELIVERED_STATUS = "Objeto entregue ao destinatário";

        private string _code;

        public Package(string code)
        {
            SetCode(code);
            TrackingHistory = new List<PackageTracking>();
        }

        #region Properties

        public string Code { get { return _code; } }
        public IList<PackageTracking> TrackingHistory { get; private set; }
        public PackageTracking LastStatus { get { return TrackingHistory.FirstOrDefault(); } }
        public DateTime? DeliveryDate { get { return IsDelivered ? GetDeliveryStatus().Date : default(DateTime?); } }
        public DateTime? ShipDate { get { return TrackingHistory.Last().Date; } }
        public bool IsDelivered { get { return IsValid && GetDeliveryStatus() != null; } }
        public bool IsValid { get { return TrackingHistory.Any(); } }

        #endregion

        #region Methods

        private void SetCode(string code)
        {
            if (string.IsNullOrEmpty(code) || code.Length != 13)
                throw new ArgumentException("Código de objeto inválido.");

            _code = code;
        }

        private void AddTrackingInfo(PackageTracking tracking)
        {
            TrackingHistory.Add(tracking);
        }

        private void AddTrackingInfo(IEnumerable<PackageTracking> list)
        {
            foreach (var item in list)
            {
                AddTrackingInfo(item);
            }
        }

        private PackageTracking GetDeliveryStatus()
        {
            return TrackingHistory.FirstOrDefault(t => t.Status.Equals(DELIVERED_STATUS));
        }

        public override string ToString()
        {
            return Code;
        }

        #endregion

        #region Parse Methods

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
            package.AddTrackingInfo(ParseTrackingInfo(dom));
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

        private static IEnumerable<PackageTracking> ParseTrackingInfo(CQ dom)
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

    public class PackageTracking
    {
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string Details { get; set; }

        public override string ToString()
        {
            return string.Format("{0:dd/MM/yyyy HH:mm} - {1} - {2}", Date, Location, Status);
        }

    }
}
