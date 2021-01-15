using ECommerce.Api.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrderService orderService;
        private readonly IProductsService productsService;
        private readonly ICustomersService customersService;

        public SearchService(IOrderService orderService, IProductsService productsService, ICustomersService customersService)
        {
            this.orderService = orderService;
            this.productsService = productsService;
            this.customersService = customersService;
        }
        public async Task<(bool IsSuccess, dynamic SearchResults)> SearchAsync(int customerId)
        {
            var orderResult = await orderService.GetOrdersAsync(customerId);
            var productsResult = await productsService.GetProductsAsync();
            var customersResult = await customersService.GetCustomersAsync(customerId);

            if(orderResult.IsSuccess)
            {
                foreach(var order in orderResult.Orders)
                {
                    foreach(var item in order.Items)
                    {
                        item.ProductName = productsResult.IsSuccess ? productsResult.Products.FirstOrDefault(p => p.Id == item.ProductId)?.Name : 
                            "Product information is not available";
                    }
                }
                var result = new
                {
                    Customer = customersResult.IsSuccess ? customersResult.Customer : new { Name = "Customer information is not available" },
                    Orders = orderResult.Orders
                };
                return (true, result);
            }
            return (false, null);
        }

    }
}
