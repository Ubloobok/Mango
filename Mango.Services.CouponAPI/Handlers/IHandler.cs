namespace Mango.Services.CouponAPI.Handlers
{
    public interface IHandler<in TIn, out TOut>
    {
        TOut Handle(TIn input);
    }
}
