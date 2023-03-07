using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		[BindProperty]
		public OrderViewModel OrderVM { get; set; }

		private readonly IUnitOfWork _unitOfWork;

		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

        public IActionResult Details(int orderId)
        {
			OrderVM = new OrderViewModel()
			{
				OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId, includeProperties:"ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderId, includeProperties:"Product"),
			};

            return View(OrderVM);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetails()
		{
			var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, tracked:false);
			orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
			orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
			orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
			orderHeaderFromDb.City = OrderVM.OrderHeader.City;
			orderHeaderFromDb.Country = OrderVM.OrderHeader.Country;
			orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

			if(OrderVM.OrderHeader.Carrier != null)
			{
				orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
			}

			if (OrderVM.OrderHeader.TrackingNumber != null)
			{
				orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			}

			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.Save();

			TempData["success"] = "Order Details Updated Successfully.";
			return RedirectToAction("Details", "Order", new { orderId = orderHeaderFromDb.Id});
		}

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeaders;

			if(User.IsInRole(SD.ROLE_ADMIN) || User.IsInRole(SD.ROLE_EMPLOYEE))
			{
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            }

			switch(status)
			{
				case "inprocess":
					orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PAYMENT_STATUS_DELEYED_PAYMENT);
                    break;
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.STATUS_IN_PROCESS);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.STATUS_SHIPPED);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.STATUS_APPROVED);
                    break;
                default:
                    break;
            }

			return Json(new { data = orderHeaders });
		}
		#endregion
	}
}
