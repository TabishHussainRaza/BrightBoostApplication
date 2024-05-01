using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Utility;
using static WebApplication.Enums.Enums;

namespace WebApplication.Controllers
{
    [ApiController]
    public class CustomerOrdersController : ControllerBase
    {
        private const double EarthRadiusKm = 6371.0;
        private readonly ApplicationDbContext _context;
        private readonly IDbConnection _dapper;


        public CustomerOrdersController(ApplicationDbContext context, DataAccess dapper)
        {
            _context = context;
            _dapper = dapper.GetConnection();
        }

        public class GetGroceryVM
        {
            public string UserID { get; set; }
            public List<SubTypeVM> Subtypes { get; set; }
        }

        public class SubTypeVM
        {
            public int SubTypeId { get; set; }
            public int Quantity { get; set; }
            public IEnumerable<Product> Products { get; set; }
        }

        public class Bundles
        {
            public List<Bundle> AllBundles { get; set; }
        }

        public class Bundle
        {
            public int Id { get; set; }
            public List<ProductDetails> Products { get; set; }
        }

        public class ProductDetails
        {
            public Product ProductInfo { get; set; }
            public Branch Branch { get; set; }
            public string Retailer { get; set; }
        }
        public class PlaceOrder
        {
            public string UserID { get; set; }
            public List<ProductList> Products { get; set; }
            public string TransactionType { get; set; }
        }

        public class ProductList
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

        [HttpGet]
        [Route("api/Order/GetOrders")]
        public async Task<IActionResult> GetAll(string UserId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(UserId))
                {
                    var customer = await _context.Customers
                        .Where(cus => cus.UserId == UserId)
                        .Include(cus => cus.Orders)
                        .FirstOrDefaultAsync();

                    if (customer != null)
                    {
                        var orderIds = customer.Orders.Select(ord => ord.Id).ToList();
                        var orders = _context.Orders.Where(ord => orderIds.Contains(ord.Id)).Include(ord=>ord.OrderItems).Select(order => new 
                        {
                            OrderStatus = order.Status,
                            OrderId = order.Id,
                            OrderItems = order.OrderItems.Select(item => new
                            {
                                OrderItemID = item.Id,
                                Item = _context.Products.Where(prod=> prod.Id == (_context.Inventory.Where(inv => inv.Id == item.InventoryId).Select(i => i.ProductId).FirstOrDefault())).FirstOrDefault(),
                                ItemQuantity = item.Quantity,
                            }).ToList(),
                            OrderTransaction = "Paid"
                        }).ToList();

                        return Ok(orders);
                    }
                    return NotFound();
                }
                else
                {
                    return Unauthorized(new { message = "Not able to access" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error" });
            }
        }

        [HttpGet]
        [Route("api/Groceries/GetMyGroceries")]
        public async Task<IActionResult> GetMyGroceries([FromBody] GetGroceryVM model)
        {
            var customer = _context.Customers.FirstOrDefault(cus => cus.UserId == model.UserID);
            if (customer != null)
            {
                var address = _context.Addresses.Where(add => add.CustomerId == customer.Id).FirstOrDefault();
                if (address != null)
                {
                    var allNearbyBranch = new List<Branch>();
                    var branches = _context.Branches.ToList();
                    foreach (var branch in branches)
                    {
                        var distance = CalculateDistance(branch.Latitude, branch.Longitude, address.Latitude, address.Longitude);
                        if (distance < 10)
                        {
                            allNearbyBranch.Add(branch);
                        }
                    }

                    var bundles = new Bundles()
                    {
                        AllBundles = new List<Bundle>{
                        new Bundle() { Id = 1, Products = new List<ProductDetails>() },
                        new Bundle() { Id = 2, Products = new List<ProductDetails>() },
                        new Bundle() { Id = 3 , Products = new List < ProductDetails >()}
                    }
                    };

                    foreach (var subType in model.Subtypes)
                    {

                        subType.Products = await _dapper.QueryAsync<Product>($@"
                                            SELECT TOP 3 p.*
                                            FROM Products p
                                            LEFT JOIN Inventory inv ON p.Id = inv.ProductId
                                            WHERE p.CategorySubTypeId = @subTypeId
                                                AND inv.BranchId IN @branchIds
                                                AND (inv.Quantity IS NOT NULL AND inv.Quantity >= @subTypeQuantity)
                                            ORDER BY p.Price;", new { branchIds = allNearbyBranch.Select(br => br.Id).ToList(), subTypeId = subType.SubTypeId, subTypeQuantity = subType.Quantity });

                        if (subType.Products.Any())
                        {
                            var productList = subType.Products.ToList();
                            for (int i = 0; i < subType.Products.Count(); i++)
                            {
                                bundles.AllBundles[i].Products.Add(new ProductDetails()
                                {
                                    ProductInfo = productList[i],
                                    Retailer = _context.Retailers.Where(ret => ret.Id == productList[i].GroupId).Select(ret => ret.Name).FirstOrDefault(),
                                });
                            }
                        }
                    }
                    return Ok(bundles);
                }
            }
            return BadRequest(new { message = "Error" });
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        // Calculates the distance between two points specified by latitude and longitude in kilometers
        public static double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            double p = Math.PI / 180.0;

            double a = 0.5 - Math.Cos((double)(lat2 - lat1) * p) / 2
                          + Math.Cos((double)lat1 * p) * Math.Cos((double)lat2 * p) *
                            (1 - Math.Cos((double)(lon2 - lon1) * p)) / 2;

            return 2 * EarthRadiusKm * Math.Asin(Math.Sqrt(a));
        }

        [HttpPost]
        [Route("api/Groceries/PlaceOrder")]
        public async Task<IActionResult> CompletePlaceOrder([FromBody] PlaceOrder model)
        {
            var customer = _context.Customers.Where(cus => cus.UserId == model.UserID).Include(cus => cus.Orders).FirstOrDefault();
            if (customer != null)
            {
                var address = _context.Addresses.Where(add => add.CustomerId == customer.Id).FirstOrDefault();
                if (address != null)
                {
                    var transaction = new Transaction()
                    {
                        Customer = customer,
                        TransactionStatus = (int)Status.Paid, //Paid
                        IsActive = true,
                        CreatedDateTime = DateTime.Now,
                    };
                    _context.Add(transaction);
                    _context.SaveChanges();

                    var order = new Order
                    {
                        Address = address,
                        CreatedDateTime = DateTime.UtcNow,
                        LastUpdateDateTime = DateTime.UtcNow,
                        Status = (int)Status.Started, //Started
                        TransactionId = transaction.Id,
                        OrderItems = new List<OrderItem>()
                    };

                    foreach (var product in model.Products)
                    {
                        var orderItem = new OrderItem();
                        var prod = _context.Products.FirstOrDefault(pr => pr.Id == product.ProductId);
                        if(prod != null)
                        {
                            var inventory = _context.Inventory.FirstOrDefault(pr => pr.ProductId == product.ProductId);
                            if (inventory != null && inventory.Quantity>=product.Quantity)
                            {
                                orderItem.LastUpdateDateTime = DateTime.UtcNow;
                                orderItem.CreatedDateTime = DateTime.UtcNow;
                                orderItem.InventoryId = inventory.Id;
                                orderItem.Inventory = inventory;
                                orderItem.IsActive = true;
                                orderItem.Quantity = product.Quantity;

                                order.OrderItems.Add(orderItem);

                                inventory.Quantity = inventory.Quantity - product.Quantity;
                            }
                        }
                    }

                    _context.Add(order);
                    _context.SaveChanges();

                    customer.Orders.Add(order);
                    _context.SaveChanges();
                    return Ok();
                }
            }
            return BadRequest(new { message = "Error" });
        }

    }

}
