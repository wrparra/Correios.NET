using System;
using System.Collections.Generic;
using System.Linq;
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
            get { return Statuses.Any() && CurrentStatus.Situation.Equals("Entregue"); }
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

        public static Package Parse(string html)
        {
            CQ dom = html;
            var packageCode = ParsePackageCode(dom);
            var package = new Package(packageCode);
            PackageStatus status = null;

            var tableRows = dom.Select("table tr");
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

            return package;
        }

        private static string ParsePackageCode(CQ dom)
        {
            var title = dom["body b"].First().Text();
            if (string.IsNullOrEmpty(title))
                return string.Empty;
            
            var titleArray = title.Split(new[] {'-'});
            return titleArray.Length > 0 ? titleArray[0].Replace("\n", string.Empty).Trim() : string.Empty;
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
