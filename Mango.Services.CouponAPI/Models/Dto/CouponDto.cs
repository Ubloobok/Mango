namespace Mango.Services.CouponAPI.Models.Dto
{
    public class CouponDto
    {
        //public CouponDto(Coupon source)
        //{
        //    this.CouponId = source.CouponId;
        //    this.CouponCode = source.CouponCode;
        //    this.DiscountAmount = source.DiscountAmount;
        //    this.MinAmount = source.MinAmount;
        //}

        public int CouponId { get; set; }
        public string? CouponCode { get; set; }
        public double DiscountAmount { get; set; }
        public int MinAmount { get; set; }

        public Coupon Map() => new()
        {
            CouponId = this.CouponId,
            CouponCode = this.CouponCode,
            DiscountAmount = this.DiscountAmount,
            MinAmount = this.MinAmount,
        };
    }

    public static class CouponDtoExtensions
    {
        public static CouponDto Map(this Coupon source) => new()
        {
            CouponId = source.CouponId,
            CouponCode = source.CouponCode,
            DiscountAmount = source.DiscountAmount,
            MinAmount = source.MinAmount,
        };

        //public static Coupon? Map(this CouponDto source) => source == null ? null : new()
        //{
        //    CouponId = source.CouponId,
        //    CouponCode = source.CouponCode,
        //    DiscountAmount = source.DiscountAmount,
        //    MinAmount = source.MinAmount,
        //};
    }
}
