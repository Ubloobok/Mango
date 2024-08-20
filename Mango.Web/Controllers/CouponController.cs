using Mango.Web.Models;
using Mango.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _couponService.GetCouponsAsync();
            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Error ?? "Invalid Result";
                return View(Enumerable.Empty<CouponDto>());
            }

            var coupons = response.Result.ToList();

            return View(coupons);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CouponDto coupon)
        {
            if (!ModelState.IsValid)
            {
                return View(coupon);
            }

            var response = await _couponService.CreateCouponAsync(coupon);
            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Error;
                return View(coupon);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _couponService.GetCouponAsync(id);
            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Error;
            }

            return View(response.Result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CouponDto coupon)
        {
            if (!ModelState.IsValid)
            {
                return View(coupon);
            }

            var response = await _couponService.UpdateCouponAsync(coupon);
            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Error ?? "Invalid Result";
                return View(coupon);
            }

            // Clear model state to show updated fields.
            ModelState.Clear();
            TempData["Success"] = "Saved";
            return View(response.Result);
            // We don't need redirection as we Clear model state.
            //return RedirectToAction(nameof(Edit), coupon.CouponId);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _couponService.DeleteCouponAsync(id);
            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Error ?? "Invalid Result";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
