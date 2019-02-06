using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WhatSAP.Helpers;
using WhatSAP.Models;
using WhatSAP.Models.Cart;

namespace WhatSAP.Controllers
{
    public class CartController : Controller
    {
        private readonly WhatSAPContext _context;

        public CartController(WhatSAPContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<CartViewModel>>(HttpContext.Session, "cart");
            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(x => x.NumOfPeople * x.Activity.Price);
            return View(cart);
        }

        public IActionResult Save(long activityId)
        {
            if (SessionHelper.GetObjectFromJson<List<CartViewModel>>(HttpContext.Session, "cart") == null)
            {
                List<CartViewModel> cart = new List<CartViewModel>();
                cart.Add(new CartViewModel { NumOfPeople = 1, Activity = _context.Activity.FirstOrDefault(x => x.ActivityId == activityId) });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<CartViewModel> cart = SessionHelper.GetObjectFromJson<List<CartViewModel>>(HttpContext.Session, "cart");
                int index = IsExist(activityId);
                if (index != -1)
                {
                    cart[index].NumOfPeople++;
                }
                else
                {
                    cart.Add(new CartViewModel() { NumOfPeople = 1, Activity = _context.Activity.FirstOrDefault(x => x.ActivityId == activityId) });
                }

                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Remove(long activityId)
        {
            List<CartViewModel> cart = SessionHelper.GetObjectFromJson<List<CartViewModel>>(HttpContext.Session, "cart");
            int index = IsExist(activityId);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);

            return RedirectToAction("Index");
        }

        private int IsExist(long id)
        {
            List<CartViewModel> cart = SessionHelper.GetObjectFromJson<List<CartViewModel>>(HttpContext.Session, "cart");

            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Activity.ActivityId == id)
                {
                    return i;
                }
            }

            return -1;
        }

        public IActionResult Checkout()
        {
            List<CartViewModel> cart = SessionHelper.GetObjectFromJson<List<CartViewModel>>(HttpContext.Session, "cart");
            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(x => x.NumOfPeople * x.Activity.Price) * 1.13;
            return View(new PaymentViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([Bind("CustomerId,Amount,NameOnCard,CardNumber,Expiration,CVV")]PaymentViewModel paymentViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            List<CartViewModel> cart = SessionHelper.GetObjectFromJson<List<CartViewModel>>(HttpContext.Session, "cart");

            PaymentMethod method = new PaymentMethod();
            method.CardNumber = paymentViewModel.CardNumber;
            method.NameOnCard = paymentViewModel.NameOnCard;
            method.Expiration = paymentViewModel.Expiration;
            method.Cvv = paymentViewModel.CVV;
            method.CustomerId = paymentViewModel.CustomerId;

            Payment payment = new Payment();
            payment.CustomerId = paymentViewModel.CustomerId;
            payment.MethodCode = method.MethodCode;
            payment.PaymentDate = DateTime.Now;
            payment.PaymentAmount = paymentViewModel.Amount;
            payment.DiscountAmount = 5;

            _context.PaymentMethod.Add(method);
            _context.Payment.Add(payment);

            await _context.SaveChangesAsync();

            return View("Completed");
        }

        public IActionResult Completed()
        {
            return View();
        }
    }
}