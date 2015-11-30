using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LTI.API.Client.Helpers;
using LTI.API.Model.Client;
using LTI.API.Model.Reference;
using Messages = LTI.API.Model.Messages;

namespace LTI.API.Client.Jobs
{
    internal class ConfirmPackListsFromCsvFile
    {
        internal void Run()
        {
            Directory.GetFiles(Environment.CurrentDirectory, "*.csv").ToList().ForEach(ProcessCsvFile);
        }

        private void ProcessCsvFile(string filePath)
        {
            var fileContents = File.ReadAllBytes(filePath);
            var csvContents = ParseCsv(fileContents);
            var headerIndexes = ParseHeaderIndexes(csvContents.First());
            var gateway = APIHelper.GetTradevineGateway();
            var salesOrders = gateway.Sales.GetSalesOrders(1, Constants.CVs.SalesOrderStatus.Values.AwaitingShipment, null, null);
            foreach (var row in csvContents.Skip(1))
            {
                ConfirmPackList(gateway, salesOrders.List, headerIndexes, row);
            }

            File.Move(filePath, filePath + ".processed");
        }

        private void ConfirmPackList(TradevineGateway gateway, IList<Messages.SalesOrder> salesOrders, Dictionary<string, int> headerIndexes, List<string> row)
        {
            string packListID = string.Empty;

            try
            {
                packListID = row[headerIndexes["PackListID"]];
                packListID = packListID.Replace("ID:", string.Empty);
                var id = !string.IsNullOrEmpty(packListID) ? long.Parse(packListID) : (long?)null;
                var courier = row[headerIndexes["Courier"]];
                var trackingReference1 = row[headerIndexes["TrackingReference1"]];
                var trackingReference2 = row[headerIndexes["TrackingReference2"]];

                var salesOrder = salesOrders.FirstOrDefault(x => x.PackLists.Any(y => y.PackListID == id));
                if (null == salesOrder)
                    return;

                var packList = salesOrder.PackLists.Single(x => x.PackListID == id);
                packList.Courier = !string.IsNullOrEmpty(courier) ? int.Parse(courier) : (int?)null;
                packList.TrackingReference = trackingReference1;
                packList.TrackingReference2 = trackingReference2;

                packList.ConfirmOptions = new Messages.ConfirmOptions() { IsInvoiceEmailed = true, IsPackingSlipEmailed = true };

                var output = gateway.Sales.ConfirmPackList(packList);

                Console.WriteLine("Packlist {0} now has status {1}", output.PackListNumber, output.Status);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error confirming packlist {0} : {1} : {2}", packListID, ex.Message, ex.StackTrace);
            }
        }

        private List<List<string>> ParseCsv(byte[] input, char delimiter = ',')
        {
            using (var ms = new MemoryStream(input))
            using (var csv = new LumenWorks.Framework.IO.Csv.CsvReader(new StreamReader(ms), false, delimiter))
            {
                var output = new List<List<string>>();
                while (csv.ReadNextRecord())
                {
                    var row = new List<string>();
                    for (int i = 0; i < csv.FieldCount; i++)
                    {
                        row.Add(csv[i]);
                    }

                    output.Add(row);
                }

                return output;
            }
        }

        private Dictionary<string, int> ParseHeaderIndexes(List<string> header)
        {
            var output = new Dictionary<string, int>();
            int index = 0;
            foreach (var item in header)
            {
                output[item] = index++;
            }

            return output;
        }
    }
}
