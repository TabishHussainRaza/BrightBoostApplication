using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Models.ViewModel;
using WebApplication.Utility;
using static NuGet.Packaging.PackagingConstants;
using static WebApplication.Enums.Enums;

namespace WebApplication.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbConnection _dapper;

        public OrdersController(ApplicationDbContext context, DataAccess dapper)
        {
            _context = context;
            _dapper = dapper.GetConnection();
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var groupId = _context.Users.Where(user => user.Id == userId).Select(us => us.GroupId).FirstOrDefault();
            var orders = new List<Order>();

            if (User.IsInRole("Administrator"))
            {
                orders = await _context.Orders.Include(o => o.Address).Include(o => o.Transaction).ToListAsync();
            }
            else if (User.IsInRole("Staff") || User.IsInRole("Branch Administrator"))
            {
                var staffCheck = _context.Staffs.Where(st => st.UserId == userId).FirstOrDefault();

                if (staffCheck != null)
                {
                    var ieOrders = await _dapper.QueryAsync<Order>(@$"SELECT DISTINCT od.* FROM Orders od 
							LEFT JOIN OrderItems item ON item.OrderId = od.Id  
							LEFT JOIN Inventory inv ON inv.Id = item.InventoryId
							LEFT JOIN Branches ibr ON ibr.Id = inv.BranchId
							WHERE ibr.RetailerId = @groupId AND ibr.Id = @Id AND od.status != 1", new { Id = staffCheck.BranchId, groupId = groupId });
                    orders = ieOrders.ToList();
                }
            }


            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var groupId = _context.Users.Where(user => user.Id == userId).Select(us => us.GroupId).FirstOrDefault();
            var dictionary = new Dictionary<int, OrderVM>();

            if (User.IsInRole("Administrator"))
            {
                var query = @$"SELECT od.*, userd.firstName AS FirstName, userd.lastName AS LastName,
		                            userd.Email, 
                                    adds.FormattedAddress,
		                            item.*, prd.* 
                            FROM Orders od 
                            LEFT JOIN OrderItems item ON item.OrderId = od.Id
                            LEFT JOIN Customers cus ON od.CustomerId = cus.Id
                            LEFT JOIN Addresses adds ON adds.CustomerId = cus.Id
                            LEFT JOIN AspNetUsers userd ON userd.Id = cus.UserId
                            LEFT JOIN Inventory inv ON inv.Id = item.InventoryId
                            LEFT JOIN Products prd ON prd.Id = inv.ProductId WHERE od.Id = @Id";
                var order = await _dapper.QueryAsync<OrderVM, OrderItem, Product, OrderVM>(
                            sql: query,
                            map: (order, orderItem, product) =>
                            {
                                if (!dictionary.TryGetValue(order.Id, out OrderVM orderEntry))
                                {
                                    orderEntry = order;
                                    dictionary.Add(order.Id, orderEntry);
                                }

                                if (orderItem != null)
                                {
                                    var orderPItem = new OrderProduct()
                                    {
                                        Id = orderItem.Id,
                                        Quantity = orderItem.Quantity,
                                    };

                                    if (product != null)
                                    {
                                        orderPItem.ProductDetails = product;
                                    }

                                    orderEntry.OrderItems.Add(orderPItem);
                                }
                                return orderEntry;
                            },
                            param: new { Id = id.Value },
                            splitOn: "Id");
            }
            else if (User.IsInRole("Staff") || User.IsInRole("Branch Administrator"))
            {
                var staffCheck = _context.Staffs.Where(st => st.UserId == userId).FirstOrDefault();

                if (staffCheck != null)
                {
                    var query = @$"SELECT od.*, userd.firstName AS FirstName, userd.lastName AS LastName,
		                            userd.Email, 
                                    adds.FormattedAddress,
		                            item.*, prd.* 
                            FROM Orders od 
                            LEFT JOIN OrderItems item ON item.OrderId = od.Id
                            LEFT JOIN Customers cus ON od.CustomerId = cus.Id
                            LEFT JOIN Addresses adds ON adds.CustomerId = cus.Id
                            LEFT JOIN AspNetUsers userd ON userd.Id = cus.UserId
                            LEFT JOIN Inventory inv ON inv.Id = item.InventoryId
                            LEFT JOIN Products prd ON prd.Id = inv.ProductId
                            LEFT JOIN Branches ibr ON ibr.Id = inv.BranchId
							WHERE ibr.RetailerId = @GroupId AND ibr.Id = @BranchId AND od.Id = @Id";

                    var order = await _dapper.QueryAsync<OrderVM, OrderItem, Product, OrderVM>(
                                sql: query,
                                map: (order, orderItem, product) =>
                                {
                                    if (!dictionary.TryGetValue(order.Id, out OrderVM orderEntry))
                                    {
                                        orderEntry = order;
                                        dictionary.Add(order.Id, orderEntry);
                                    }

                                    if (orderItem != null)
                                    {


                                        if (product != null && product.GroupId == groupId)
                                        {
                                            var orderPItem = new OrderProduct()
                                            {
                                                Id = orderItem.Id,
                                                Quantity = orderItem.Quantity,
                                                ProductDetails = product
                                            };
                                            orderEntry.OrderItems.Add(orderPItem);
                                        }

                                    }
                                    return orderEntry;
                                },
                                param: new { Id = id.Value, BranchId = staffCheck.BranchId, GroupId = groupId.Value },
                                splitOn: "Id");

                    var currentOrder = dictionary.Values.FirstOrDefault();

                    if(currentOrder != null && currentOrder.Status == (int)Status.Started)
                    {
                        dictionary.Clear();
                    }
                }
            }

            if (dictionary.Values.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(dictionary.Values.FirstOrDefault());
        }

        [HttpPost]
        public async Task<JsonResult> Save(int orderId)
        {
            var storingState = 0;

            if (orderId != 0)
            {
                var existing = _context.Orders.Where(ord => ord.Id == orderId).FirstOrDefault();

                if (existing == null)
                {
                    return Json(false);
                }

                if (existing.Status != (int)Status.Complete)
                {
                    existing.Status = existing.Status + 1;
                    existing.LastUpdateDateTime = DateTime.UtcNow;
                }
                _context.Entry(existing).State = EntityState.Modified;
                storingState = await _context.SaveChangesAsync();
                return Json(storingState);
            }
            return Json(storingState);
        }
    }
}
