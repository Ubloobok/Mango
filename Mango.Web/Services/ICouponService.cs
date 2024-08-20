using Mango.Web.Models;

namespace Mango.Web.Services
{
    public interface ICouponService
    {
        Task<ResponseDto<IEnumerable<CouponDto>>> GetCouponsAsync();
        Task<ResponseDto<CouponDto>> GetCouponAsync(int couponId);
        Task<ResponseDto<CouponDto>> GetCouponAsync(string couponCode);
        Task<ResponseDto<CouponDto>> CreateCouponAsync(CouponDto coupon);
        Task<ResponseDto<CouponDto>> UpdateCouponAsync(CouponDto coupon);
        Task<ResponseDto<CouponDto>> DeleteCouponAsync(int couponId);
    }
}
