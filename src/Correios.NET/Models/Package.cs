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

        public void AddTrackingInfo(PackageTracking tracking)
        {
            TrackingHistory.Add(tracking);
        }

        public void AddTrackingInfo(IEnumerable<PackageTracking> list)
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
