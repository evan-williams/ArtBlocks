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
    internal class CreateSalesOrdersFromOpencartCsvFile
    {
        internal void Run()
        {
            Directory.GetFiles(Environment.CurrentDirectory, "*.csv").ToList().ForEach(ProcessCsvFile);
        }

        /******************* Work in Progress - Not Finished *************************/

        private void ProcessCsvFile(string filePath)
        {
            var fileContents = File.ReadAllBytes(filePath);
            var csvContents = ParseCsv(fileContents);
            var headerIndexes = ParseHeaderIndexes(csvContents.First());
            var gateway = APIHelper.GetTradevineGateway();
            foreach (var row in csvContents.Skip(1))
            {
                CreateSalesOrder(gateway, headerIndexes, row);
            }

            File.Move(filePath, filePath + ".processed");
        }

        private void CreateSalesOrder(TradevineGateway gateway, Dictionary<string, int> headerIndexes, List<string> row)
        {
            string orderID = string.Empty;

            try
            {
                var salesOrder = ParseSalesOrderFromRow(headerIndexes, row);

                var output = gateway.Sales.SaveSalesOrder(null, salesOrder);

                Console.WriteLine("Sales Order {0} with reference {1} saved to Tradevine {1}", output.OrderNumber, output.CustomerOrderReference);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating sales order {0} : {1} : {2}", orderID, ex.Message, ex.StackTrace);
            }
        }

        private Messages.SalesOrder ParseSalesOrderFromRow(Dictionary<string, int> headerIndexes, List<string> row)
        {
            var salesOrder = new Messages.SalesOrder
            {
                Customer = new Messages.Customer(),
                BillingAddress = new Messages.Address(),
                ShippingAddress = new Messages.Address()
            };

            salesOrder.CustomerOrderReference= row[headerIndexes["order_id"]];
            salesOrder.GrandTotal = decimal.Parse(row[headerIndexes["total"]]);

            salesOrder.Customer.FirstName = row[headerIndexes["firstname"]];
            salesOrder.Customer.LastName = row[headerIndexes["lastname"]];
            salesOrder.Customer.Email = row[headerIndexes["email"]];

            salesOrder.BillingAddress.AddressLine1 = row[headerIndexes["payment_address_1"]];
            salesOrder.BillingAddress.AddressLine2 = row[headerIndexes["payment_address_2"]];
            salesOrder.BillingAddress.TownCity = row[headerIndexes["payment_city"]];
            salesOrder.BillingAddress.PostalCode = row[headerIndexes["payment_postcode"]];
            salesOrder.BillingAddress.RegionState = Constants.CVs.RegionsandStates.LookupByName(row[headerIndexes["payment_region"]]);
            salesOrder.BillingAddress.Country = Constants.CVs.Country.LookupByName(row[headerIndexes["payment_country"]]);

            var shippingSameAsBilling = bool.Parse(row[headerIndexes["shipping_address_same_as_billing"]]);
            if (shippingSameAsBilling)
            {
                salesOrder.ShippingAddress = salesOrder.BillingAddress;
            }
            else
            {
                salesOrder.ShippingAddress.AddressLine1 = row[headerIndexes["shipping_address_1"]];
                salesOrder.ShippingAddress.AddressLine2 = row[headerIndexes["shipping_address_2"]];
                salesOrder.ShippingAddress.TownCity = row[headerIndexes["shipping_city"]];
                salesOrder.ShippingAddress.PostalCode = row[headerIndexes["shipping_postcode"]];
                salesOrder.ShippingAddress.RegionState = Constants.CVs.RegionsandStates.LookupByName(row[headerIndexes["shipping_region"]]);
                salesOrder.ShippingAddress.Country = Constants.CVs.Country.LookupByName(row[headerIndexes["shipping_country"]]);
            }

            salesOrder.ShipmentType = Constants.CVs.ShippingType.LookupByName(row[headerIndexes["shipping_method"]]);
            salesOrder.RequestedShippingDate = DateTime.Parse(row[headerIndexes["requested_shipping_date"]]);
            var shippingCost = decimal.Parse(row[headerIndexes["order_shipping_cost"]]);
            if (shippingCost != 0)
            {
                var shippingLine = new Messages.SalesOrderLine()
                {
                    SellPriceIncTax = shippingCost,
                    Quantity = 1M,
                    ProductID = 1,
                    LineNotes = row[headerIndexes["order_shipping_description"]],
                    TaxTotal = decimal.Parse(row[headerIndexes["order_shipping_cost"]])
                };
            }

            return salesOrder;
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
