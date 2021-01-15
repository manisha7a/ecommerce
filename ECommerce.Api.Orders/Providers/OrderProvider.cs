using AutoMapper;
using ECommerce.Api.Orders.Db;
using ECommerce.Api.Orders.Interfaces;
using ECommerce.Api.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Orders.Providers
{
    public class OrderProvider : IOrderProvider
    {
        private readonly OrderDbContext dbContext;
        private readonly ILogger logger;
        private readonly IMapper mapper;

        public OrderProvider(OrderDbContext dbContext, ILogger<OrderProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            SeedData();
        }

        private void SeedData()
        {
            if(!dbContext.Orders.Any())
            {
                dbContext.Orders.Add(new Db.Order 
                { 
                    Id = 1, 
                    CustomerId = 1, 
                    OrderDate = DateTime.Now, 
                    Total = 100, 
                    Items = new List<Db.OrderItem>()
                    { 
                        new Db.OrderItem { OrderId = 1, ProductId = 1, Quantity = 100, UnitPrice = 100 } 
                    } 
                });
                dbContext.Orders.Add(new Db.Order
                {
                    Id = 2,
                    CustomerId = 1,
                    OrderDate = DateTime.Now,
                    Total = 100,
                    Items = new List<Db.OrderItem>()
                    {
                        new Db.OrderItem { OrderId = 1, ProductId = 1, Quantity = 100, UnitPrice = 100 }
                    }
                });
                dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSucess, IEnumerable<Models.Order> Orders, string ErrorMessage)> GetOrdersAsync(int customerId)
        {
            try
            {
                var orders = await dbContext.Orders
                    .Where(o => o.CustomerId == customerId)
                    .Include(o => o.Items)
                    .ToListAsync();
                if (orders.Any() && orders != null)
                {
                    var result = mapper.Map<IEnumerable<Db.Order>, IEnumerable<Models.Order>>(orders);

                    return (true, result, null);
                }
                return (false, null, "Not found!");
            }
            
            catch(Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
    }
}
