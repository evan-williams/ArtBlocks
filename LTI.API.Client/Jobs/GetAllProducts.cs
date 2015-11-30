using System;
using LTI.API.Client.Helpers;
using LTI.API.Model.Framework;
using LTI.API.Model.Messages;

namespace LTI.API.Client.Jobs
{
    internal class GetAllProducts
    {
        internal void Run()
        {
            var gateway = APIHelper.GetTradevineGateway();
            var products = gateway.Products.GetAllProducts();
            PrintToConsole(products);
        }

        private void PrintToConsole(PagedCollection<Product> products)
        {
            Console.WriteLine("{0} products found", products.TotalCount);
            foreach (var product in products.List)
            {
                Console.WriteLine("{0} - {1} - {2}", product.ProductID, product.Code, product.Name);
            }
        }
    }
}
