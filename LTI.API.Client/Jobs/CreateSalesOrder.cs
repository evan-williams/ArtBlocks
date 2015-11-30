using System;
using System.Collections.Generic;
using System.Linq;
using LTI.API.Client.Helpers;
using LTI.API.Model.Messages;
using LTI.API.Model.Reference;

namespace LTI.API.Client.Jobs
{
    internal class CreateSalesOrder
    {
        internal void Run()
        {
            var gateway = APIHelper.GetTradevineGateway();
            var products = gateway.Products.GetProducts(1);
            var salesOrder = PrepareSalesOrder(products.List.Last());
            var output = gateway.Sales.SaveSalesOrder(null, salesOrder);
            Console.WriteLine("Create sales order with ID {0} and number {1} with total {2}", output.SalesOrderID, output.OrderNumber, output.GrandTotal);
        }

        private SalesOrder PrepareSalesOrder(Product product)
        {
            var output = new SalesOrder
            {
                ApplyTaxes = true,
                CustomerOrderReference = "P1234",
                RequestedShippingDate = DateTime.Now,
                PaymentType = Constants.CVs.PaymentType.Values.BankTransfer,
                ShipmentType = Constants.CVs.ShippingType.Values.Post,
                OrderOrigin = Constants.CVs.OrderOrigin.Values.Manual,
                TermsOfTrade = Constants.CVs.TermsofTrade.Values.Immediate,
                SalesOrderLines = new List<SalesOrderLine>()
                {
                    new SalesOrderLine()
                        {
                            ProductID = product.ProductID,
                            Quantity = 5,
                            SellPrice = 1.00M,
                            SellPriceExTax = 1.00M,
                            SellPriceIncTax = 1.15M,
                            TaxCode = "GST"
                        }
                },
                Customer = new Customer()
                {
                    FirstName = "Joe",
                    LastName = "Bloggs",
                    Email = "joe@bloggs.com",
                    PhoneNumber = "021-896-435"
                },
                BillingAddress = new Address()
                {
                    AddressLine1 = "567 Lunar Crescent",
                    AddressLine2 = "Burnside",
                    TownCity = "Christchurch",
                    PostalCode = "8054",
                    RegionState = Constants.CVs.RegionsandStates.Values.Canterbury,
                    Country = Constants.CVs.Country.Values.NewZealand
                },
                ShippingAddress = new Address()
                {
                    AddressLine1 = "Warehouse 67",
                    AddressLine2 = "Parnell",
                    TownCity = "Auckland",
                    PostalCode = "5654",
                    RegionState = Constants.CVs.RegionsandStates.Values.Auckland,
                    Country = Constants.CVs.Country.Values.NewZealand
                }
            };

            return output;
        }
    }
}
