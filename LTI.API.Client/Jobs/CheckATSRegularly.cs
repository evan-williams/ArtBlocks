using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using LTI.API.Client.Helpers;
using LTI.API.Model.Client;
using LTI.API.Model.Messages;
using LTI.API.Model.Reference;

namespace LTI.API.Client.Jobs
{
    public class CheckAtsRegularly
    {
        private const int pollSecs = 30;

        public bool Stop { get; set; }

        public void Run()
        {
            Console.WriteLine("Checking ATS...");
            var gateway = APIHelper.GetTradevineGateway();

            while (!Stop)
            {
                try
                {
                    GoCheckProducts(gateway);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An exception was thrown: {0} - {1}", ex.Message, ex.StackTrace);
                    var webEx = ex as WebException;
                    var httpWebResponse = webEx?.Response as HttpWebResponse;
                    if(httpWebResponse != null)
                        Console.WriteLine(httpWebResponse.StatusDescription);
                }
                finally
                {
                    Thread.Sleep(pollSecs * 1000);
                }
            }      
        }

        private void GoCheckProducts(TradevineGateway gateway)
        {
            var products = gateway.Products.GetAllProducts();

            var boughtProductsNeedingStockReduction = products.List.Where(x => x.SoftAllocated > 0 &&
            x.PerWarehouseInventory != null && x.PerWarehouseInventory.Any(y => y.WarehouseCode == "WH2" && y.QuantityInStockSnapshot > 0)).ToList();

            boughtProductsNeedingStockReduction.ForEach(x => ReduceFakeStock(x, gateway));  
        }

        private void ReduceFakeStock(Product product, TradevineGateway gateway)
        {
            var adjustment = new ProductInventory
            {
                ProductID = product.ProductID,
                InventoryType = Constants.CVs.InventoryEntryType.Values.Stocktake,
                QuantityInStockSnapshot = 0M,
                WarehouseCode = "WH2" 
            };

            gateway.Products.AdjustProductInventory(adjustment);

            Console.WriteLine("Reducing fake stock for product " + product.Code);
        }
    }
}
