using System;
using System.Collections.Generic;
using System.Linq;
using LTI.API.Client.Helpers;
using LTI.API.Model.Client;
using LTI.API.Model.Messages;
using LTI.API.Model.Reference;

namespace LTI.API.Client.Jobs
{
    internal class ProcessPendingPacklists
    {
        internal void Run()
        {
            var gateway = APIHelper.GetTradevineGateway();
            var packLists = GetAllPendingPackLists(gateway);
            packLists.ForEach(x => UpdatePacklistToAwaitingPack(gateway, x));
        }

        private List<PackList> GetAllPendingPackLists(TradevineGateway gateway)
        {
            var output = new List<PackList>();
            var pageNumber = 1;
            var salesOrders = gateway.Sales.GetSalesOrders(pageNumber, Constants.CVs.SalesOrderStatus.Values.AwaitingShipment, null, null);
            if (!salesOrders.List.Any())
                return output;

            while (output.Count < salesOrders.TotalCount)
            {
                output.AddRange(salesOrders.List.SelectMany(x => x.PackLists).Where(y => y.Status == Constants.CVs.PackListStatus.Values.Pending));
                if (salesOrders.TotalCount > output.Count)
                {
                    salesOrders = gateway.Sales.GetSalesOrders(++pageNumber, Constants.CVs.SalesOrderStatus.Values.AwaitingShipment, null, null);
                }
            }

            return output;
        }

        private PackList UpdatePacklistToAwaitingPack(TradevineGateway gateway, PackList input)
        {
            input.Status = Constants.CVs.PackListStatus.Values.AwaitingPack;
            var output = gateway.Sales.UpdatePackList(input);

            Console.WriteLine("Packlist {0} now has status {1}", output.PackListNumber, output.Status);

            return output;
        }
    }
}
