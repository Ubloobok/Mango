using Mango.Web.Models;

namespace Mango.Web.Services
{
    public class CouponService : ICouponService
    {
        public class Configuration
        {
            public string CouponAPI { get; set; }
        }

        private readonly IBaseService _baseService;
        private readonly Configuration _configuration;

        public CouponService(IBaseService baseService, Configuration configuration)
        {
            _baseService = baseService;
            _configuration = configuration;
        }

        public async Task<ResponseDto<IEnumerable<CouponDto>>> GetCouponsAsync()
        {
            return await _baseService.SendAsync<IEnumerable<CouponDto>>(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = _configuration.CouponAPI + "/api/coupons"
            });
        }

        public async Task<ResponseDto<CouponDto>> GetCouponAsync(int couponId)
        {
            return await _baseService.SendAsync<CouponDto>(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = _configuration.CouponAPI + "/api/coupons/" + couponId
            });
        }

        public async Task<ResponseDto<CouponDto>> GetCouponAsync(string couponCode)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto<CouponDto>> CreateCouponAsync(CouponDto coupon)
        {
            return await _baseService.SendAsync<CouponDto>(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = _configuration.CouponAPI + "/api/coupons",
                Data = coupon
            });
        }

        public async Task<ResponseDto<CouponDto>> UpdateCouponAsync(CouponDto coupon)
        {
            return await _baseService.SendAsync<CouponDto>(new RequestDto
            {
                ApiType = ApiType.PUT,
                Url = _configuration.CouponAPI + "/api/coupons",
                Data = coupon
            });
        }

        public async Task<ResponseDto<CouponDto>> DeleteCouponAsync(int couponId)
        {
            return await _baseService.SendAsync<CouponDto>(new RequestDto
            {
                ApiType = ApiType.DELETE,
                Url = _configuration.CouponAPI + "/api/coupons/" + couponId
            });
        }
    }
}
