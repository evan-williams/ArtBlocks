using System;
using System.IO;
using System.Linq;
using LTI.API.Client.Helpers;
using LTI.API.Model.Messages;

namespace LTI.API.Client.Jobs
{
    internal class CreateShopifyProduct
    {
        private const string storeDomainName = "evan-test2.myshopify.com"; // Change this to your own store domain

        internal void Run()
        {
            var gateway = APIHelper.GetTradevineGateway();

            var photo = PreparePhoto();
            var outputPhoto = gateway.Photos.UploadPhoto(photo);
            Console.WriteLine("Uploaded photo {0} with ID {1}", outputPhoto.FileName, outputPhoto.PhotoID);

            var product = PrepareProduct();
            var output = gateway.Products.SaveProduct(null, product);
            Console.WriteLine("Created product with ID {0} and code {1}", output.ProductID, output.Code);

            var shopifyProduct = PrepareShopifyProduct(output);
            var outputShopifyProduct = gateway.Products.SaveShopifyProduct(null, shopifyProduct);
            Console.WriteLine("Created Shopify product with ID {0} and title {1}", outputShopifyProduct.ShopifyProductID, outputShopifyProduct.Title);

            var readByID = gateway.Products.GetShopifyProductById(outputShopifyProduct.ShopifyProductID ?? 0);
            Console.WriteLine("Read Shopify product with ID {0} and title {1} using ID", readByID.ShopifyProductID, readByID.Title);

            var readByCodeAndStoreDomain = gateway.Products.GetShopifyProducts(1, storeDomainName, output.Code).List.Single();
            Console.WriteLine("Read Shopify product with ID {0} and title {1} using code and store name", readByCodeAndStoreDomain.ShopifyProductID, readByCodeAndStoreDomain.Title);
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

        private ShopifyProduct PrepareShopifyProduct(Product product)
        {
            return new ShopifyProduct()
            {
                ShopifyStoreDomain = storeDomainName,
                ProductCode = product.Code,
                Title = "Some new cool product on Shopify",
                BodyHtml = "<h1>Terminator Doll</h1>",
                OptionName1 = "Color",
                OptionValue1 = "Red",
                SellPriceIncTax = 5.99M,
                Vendor = "Adidas",
                ProductType = "Dolls",
                Handle = "term-doll",
                IsListedOnShopify = true,
                IsPublished = true,
                PhotoIdentifier = "widget.jpg"
            };
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
