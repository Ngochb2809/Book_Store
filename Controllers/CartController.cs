using Book_Store.Data;
using Book_Store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.PowerBI.Api.Models;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;


namespace Book_Store.Controllers
{
    public class CartController : Controller
    {
        private ApplicationDbContext _db;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // GET: Shop
        public CartController(ApplicationDbContext db, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this._db = db;
            this._context = context;
            this._userManager = userManager;
        }
        [Authorize]
        public IActionResult Index()
        {
            var _book = getAllBook();
            ViewBag.book = _book;
            return View();
        }
        
        public List<Book> getAllBook()
        {
            return _db.Book.ToList();
        }
        
        public Book getDetailBook(int id)
        {
            var book = _db.Book.Find(id);
            return book;
        }
        
        public IActionResult addCart(int id)
        {
            var cart = HttpContext.Session.GetString("cart");//get key cart
            if (cart == null)
            {
                var book = getDetailBook(id);
                List<Cart> listCart = new List<Cart>()
               {
                   new Cart
                   {
                       Book = book,
                       Quantity = 1
                   }
               };
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(listCart));

            }
            else
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                bool check = true;
                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Book.Id == id)
                    {
                        dataCart[i].Quantity++;
                        check = false;
                    }
                }
                if (check)
                {
                    dataCart.Add(new Cart
                    {
                        Book = getDetailBook(id),
                        Quantity = 1
                    });
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                // var cart2 = HttpContext.Session.GetString("cart");//get key cart
                //  return Json(cart2);
            }

            return RedirectToAction(nameof(Index));

        }
        [Authorize]
        public IActionResult ListCart()
        {
            var cart = HttpContext.Session.GetString("cart");//get key cart
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                if (dataCart.Count > 0)
                {
                    ViewBag.carts = dataCart;
                    return View();
                }
            }
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        public IActionResult updateCart(int id, int quantity)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                if (quantity > 0)
                {
                    for (int i = 0; i < dataCart.Count; i++)
                    {
                        if (dataCart[i].Book.Id == id)
                        {
                            dataCart[i].Quantity = quantity;
                        }
                    }


                    HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                }
                var cart2 = HttpContext.Session.GetString("cart");
                return Ok(quantity);
            }
            return BadRequest();

        }
        
        public IActionResult deleteCart(int id)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Book.Id == id)
                    {
                        dataCart.RemoveAt(i);
                    }
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                return RedirectToAction(nameof(ListCart));
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<ActionResult> CheckOut()
        {
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("./Login");
            }
            if (ModelState.IsValid)
            {
                var cart = HttpContext.Session.GetString("cart");
                //int count = 0;
                //if (count == null)
                //{
                //    count = _context.Order.Max(m => m.Count); ;
                //}
                if (cart != null)
                {
                    List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

                    for (int i = 0; i < dataCart.Count; i++)
                    {
                        Order order = new Order()
                        {
                            UserId = user.Id,
                            BookId = dataCart[i].Book.Id,
                            Qty = dataCart[i].Quantity,
                            Price = Convert.ToDouble(dataCart[i].Quantity * dataCart[i].Book.Price),
                            Phone = "0"

                        };
                        _context.Order.Add(order);
                        _context.SaveChanges();
                        deleteCart(dataCart[i].Book.Id);

                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> OrderList(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = _context.Order.Where(o => o.UserId == userId);

            if (User.IsInRole("Admin"))
            {
                orders = _context.Order;
            }

            var applicationDbContext = orders.Include(o => o.Book);

            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> OrderDetails(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }

}
    
