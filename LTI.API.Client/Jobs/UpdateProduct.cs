using System;
using System.Linq;
using LTI.API.Client.Helpers;

namespace LTI.API.Client.Jobs
{
    internal class UpdateProduct
    {
        internal void Run()
        {
            var gateway = APIHelper.GetTradevineGateway();
            var product = gateway.Products.GetProducts(1).List.First();
            product.InternalNotes = string.Format("This product was changed on {0}", DateTime.Now.ToString());
            var output = gateway.Products.SaveProduct(product.ProductID, product);
            Console.WriteLine("Update product with ID {0} and code {1}", output.ProductID, output.Code);
        }
    }
}
