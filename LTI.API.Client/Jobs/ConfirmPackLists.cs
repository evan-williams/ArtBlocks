using System;
using System.Linq;
using LTI.API.Client.Helpers;
using LTI.API.Model.Client;
using LTI.API.Model.Messages;
using LTI.API.Model.Reference;

namespace LTI.API.Client.Jobs
{
    internal class ConfirmPackLists
    {
        internal void Run()
        {
            var gateway = APIHelper.GetTradevineGateway();
            var packList = GetFirstAwaitingShipmentPackList(gateway);
            if (null != packList)
            {
                packList.ConfirmOptions = new ConfirmOptions() { IsInvoiceEmailed = true, IsPackingSlipEmailed = true };
                var output = gateway.Sales.ConfirmPackList(packList);
                Console.WriteLine("Packlist {0} now has status {1}", output.PackListNumber, output.Status);
            }
        }

        private PackList GetFirstAwaitingShipmentPackList(TradevineGateway gateway)
        {
            PackList output = null;
            var salesOrder = gateway.Sales.GetSalesOrders(1, Constants.CVs.SalesOrderStatus.Values.AwaitingShipment, null, null).List.FirstOrDefault();
            if (null != salesOrder)
            {
                output = salesOrder.PackLists.FirstOrDefault(x => x.Status != Constants.CVs.PackListStatus.Values.Completed);
            }

            return output;
        }
    }
}
