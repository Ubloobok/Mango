using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Handlers;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupons")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<CouponAPIController> _logger;

        public CouponAPIController(
            AppDbContext appDbContext,
            ILogger<CouponAPIController> logger)
        {
            _db = appDbContext;
            _logger = logger;
        }

        [HttpGet]
        public ResponseDto<IEnumerable<CouponDto>> Get()
        {
            //ControllerContext.ActionDescriptor.ActionName
            //using var scope = _logger.BeginScope("Method: {MethodName}", nameof(Get));
            try
            {
                _logger.LogInformation("Executing method");
                var coupons = _db.Coupons.ToList();
                return new()
                {
                    Result = coupons.Select(c => c.Map()),
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Error = ex.Message,
                    IsSuccess = false
                };
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto<CouponDto> Get(int id)
        {
            try
            {
                var coupon = _db.Coupons.FirstOrDefault(c => c.CouponId == id);
                if (coupon == null)
                {
                    return new()
                    {
                        IsSuccess = false,
                        Error = "Not found"
                    };
                }

                return new()
                {
                    Result = coupon.Map(),
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Error = ex.Message,
                    IsSuccess = false,
                };
            }
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto<CouponDto> GetByCode(string code)
        {
            try
            {
                var coupon = _db.Coupons.FirstOrDefault(c => c.CouponCode.ToLower() == code.ToLower());
                if (coupon == null)
                {
                    return new()
                    {
                        IsSuccess = false,
                        Error = "Not found"
                    };
                }

                return new()
                {
                    Result = coupon.Map(),
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Error = ex.Message,
                    IsSuccess = false,
                };
            }
        }

        [HttpPost]
        public ResponseDto<CouponDto> Post([FromBody] CouponDto couponDto)
        {
            try
            {
                var coupon = couponDto.Map();
                if (coupon == null)
                {
                    return new()
                    {
                        IsSuccess = false,
                        Error = "Empty request"
                    };
                }

                if (coupon.CouponCode.StartsWith("TEST"))
                {
                    return new()
                    {
                        IsSuccess = false,
                        Error = "No more TEST Coupons"
                    };
                }

                _db.Coupons.Add(coupon);
                _db.SaveChanges();
                return new()
                {
                    Result = coupon.Map(),
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Error = ex.Message,
                    IsSuccess = false,
                };
            }
        }

        [HttpPut]
        public ResponseDto<CouponDto> Put([FromBody] CouponDto couponDto)
        {
            try
            {
                var coupon = couponDto.Map();
                if (coupon == null)
                {
                    return new()
                    {
                        IsSuccess = false,
                        Error = "Empty request"
                    };
                }

                _db.Coupons.Update(coupon);
                _db.SaveChanges();

                return new()
                {
                    IsSuccess = true,
                    Result = coupon.Map()
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSuccess = false,
                    Error = ex.Message
                };
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public ResponseDto<CouponDto> Delete(int id)
        {
            try
            {
                var coupon = _db.Coupons.FirstOrDefault(c => c.CouponId == id);
                if (coupon == null)
                {
                    return new()
                    {
                        IsSuccess = false,
                        Error = "Not found"
                    };
                }

                if (coupon.CouponCode.StartsWith("TEST"))
                {
                    return new()
                    {
                        IsSuccess = false,
                        Error = "No permissions to delete TEST coupons"
                    };
                }

                _db.Coupons.Remove(coupon);
                _db.SaveChanges();
                return new()
                {
                    Result = coupon.Map(),
                    IsSuccess = coupon != null,
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Error = ex.Message,
                    IsSuccess = false,
                };
            }
        }
    }
}
