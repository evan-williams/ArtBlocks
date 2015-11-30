using System;
using System.Collections.Generic;
using System.Linq;
using LTI.API.Client.Helpers;
using LTI.API.Model.Client;
using LTI.API.Model.Messages;
using LTI.API.Model.Reference;

namespace LTI.API.Client.Jobs
{
    internal class PopulatePackListsWithProductDescriptions
    {
        internal void Run()
        {
            var gateway = APIHelper.GetTradevineGateway();
            var packLists = GetAllPendingPackLists(gateway);
            packLists.ForEach(x => UpdateDeliveryNotesWithProductDescriptions(gateway, x));
        }

        private List<PackList> GetAllPendingPackLists(TradevineGateway gateway)
        {
            var output = new List<PackList>();
            var pageNumber = 1;
            var salesOrdersResult = gateway.Sales.GetSalesOrders(pageNumber, Constants.CVs.SalesOrderStatus.Values.AwaitingShipment, null, null);
            if (!salesOrdersResult.List.Any())
                return output;

            var salesOrders = salesOrdersResult.List.ToList();

            do
            {
                output.AddRange(salesOrders.SelectMany(x => x.PackLists).Where(y => y.Status == Constants.CVs.PackListStatus.Values.Pending)
                    .Where(y => output.All(z => z.PackListID != y.PackListID)));

                if (salesOrdersResult.TotalCount > salesOrders.Count)
                {
                    salesOrders.AddRange(gateway.Sales.GetSalesOrders(++pageNumber, Constants.CVs.SalesOrderStatus.Values.AwaitingShipment, null, null).List);
                }
            }
            while (salesOrders.Count < salesOrdersResult.TotalCount);

            return output;
        }

        private PackList UpdateDeliveryNotesWithProductDescriptions(TradevineGateway gateway, PackList input)
        {
            var originalDeliveryNotes = input.DeliveryNotes;

            foreach (var packListItem in input.PackListItems)
            {
                string productDescription = gateway.Products.GetProductById(packListItem.ProductID.Value).Description;
                string productCode = gateway.Products.GetProductById(packListItem.ProductID.Value).Code;
                if (productCode != "SHIP")
                {
                    productDescription = productDescription.Substring(0, Math.Min(1000, productDescription.Length));

                    if (string.IsNullOrEmpty(input.DeliveryNotes) || !input.DeliveryNotes.Contains(productDescription))
                    {
                        input.DeliveryNotes = (input.DeliveryNotes ?? string.Empty) + productDescription;
                    }
                }
            }

            if (input.DeliveryNotes == originalDeliveryNotes)
                return input;

            Console.WriteLine("Updating delivery notes for {0}", input.PackListNumber);
            var output = gateway.Sales.UpdatePackList(input);

            return output;
        }
    }
}
