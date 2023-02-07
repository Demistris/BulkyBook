using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties:"Product")
            };

            foreach(var cart in ShoppingCartViewModel.ListCart)
            {
                cart.Price = GetProceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartViewModel.CartTotal += (cart.Price * cart.Count);
			}

            return View(ShoppingCartViewModel);
        }

		public IActionResult Summary()
		{
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //ShoppingCartViewModel = new ShoppingCartViewModel()
            //{
            //	ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product")
            //};

            //foreach (var cart in ShoppingCartViewModel.ListCart)
            //{
            //	cart.Price = GetProceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
            //	ShoppingCartViewModel.CartTotal += (cart.Price * cart.Count);
            //}

            //return View(ShoppingCartViewModel);

            return View();
		}

		public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

		public IActionResult Minus(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);

            if(cart.Count <= 1)
            {
				_unitOfWork.ShoppingCart.Remove(cart);
			}
            else
            {
				_unitOfWork.ShoppingCart.DecrementCount(cart, 1);
			}

			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
			_unitOfWork.ShoppingCart.Remove(cart);
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		private double GetProceBasedOnQuantity(double quantity, double price, double price50, double price100)
        {
            switch(quantity)
            {
                case <= 50:
                    return price;
                case <= 100:
                    return price50;
				case > 100:
					return price100;
                default:
                    return price;
			}
        }
    }
}
