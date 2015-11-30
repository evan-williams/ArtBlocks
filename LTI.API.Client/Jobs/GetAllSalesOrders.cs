using System;
using LTI.API.Client.Helpers;
using LTI.API.Model.Framework;
using LTI.API.Model.Messages;

namespace LTI.API.Client.Jobs
{
    internal class GetAllSalesOrders
    {
        internal void Run()
        {
            var gateway = APIHelper.GetTradevineGateway();
            var salesOrders = gateway.Sales.GetAllSalesOrders();
            PrintToConsole(salesOrders);
        }

        private void PrintToConsole(PagedCollection<SalesOrder> salesOrders)
        {
            Console.WriteLine("{0} sales orders found", salesOrders.TotalCount);
            foreach (var salesOrder in salesOrders.List)
            {
                Console.WriteLine("{0} - {1} - {2}", salesOrder.SalesOrderID, salesOrder.OrderNumber, salesOrder.Status);
            }
        }
    }
}
