using System;
using System.IO;
using LTI.API.Client.Helpers;
using LTI.API.Model.Messages;

namespace LTI.API.Client.Jobs
{
    internal class CreateProduct
    {
        internal void Run()
        {
            var gateway = APIHelper.GetTradevineGateway();

            var photo = PreparePhoto();
            var outputPhoto = gateway.Photos.UploadPhoto(photo);
            Console.WriteLine("Uploaded photo {0} with ID {1}", outputPhoto.FileName, outputPhoto.PhotoID);

            var product = PrepareProduct();
            var output = gateway.Products.SaveProduct(null, product);
            Console.WriteLine("Created product with ID {0} and code {1}", output.ProductID, output.Code);
        }

        private Product PrepareProduct()
        {
            var output = new Product
            {
                Name = "A test widget",
                Description = "Some long description about my product",
                EnableInventory = true,
                QuantityInStock = 50,
                MinimumStockQuantity = 10,
                CostPrice = 1.50M,
                SellPriceExTax = 3.00M,
                SellPriceIncTax = 3.45M,
                Barcode = "77842523525623525",
                ExternalNotes = "Some notes that appear on documentation",
                InternalNotes = "Some private notes that only I can see e.g. This product was created via the API",
                UnitOfMeasure = "Box",
                TaxCode = "GST",
                PhotoIdentifier = "widget.jpg"
            };

            return output;
        }

        private Photo PreparePhoto()
        {
            var photoBytes = File.ReadAllBytes(Environment.CurrentDirectory + @"\Resources\widget.jpg");
            var output = new Photo()
            {
                FileName = "widget.jpg",
                ContentsBase64 = Convert.ToBase64String(photoBytes)
            };

            return output;
        }
    }
}
