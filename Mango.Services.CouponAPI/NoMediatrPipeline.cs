using Mango.Services.CouponAPI.Handlers;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;

namespace Mango.Services.CouponAPI
{
    public static class NoMediatrPipeline
    {
        public static IResult Handle<TRequest, TIn, TOut, TResponse>(
            ILoggerFactory factory, 
            HttpContext http,
            IHandler<TIn, TOut> handler,
            TRequest request,
            Func<TRequest, TIn> mapIn,
            Func<TOut, TResponse> mapOut)
        {
            var logger = factory.CreateLogger(http.Request.Path);
            logger.LogInformation("Enter");
            try
            {
                var @in = mapIn(request);
                var @out = handler.Handle(@in);
                logger.LogInformation("Ok");
                return Results.Ok(new ResponseDto<TResponse>
                {
                    Result = mapOut(@out),
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching coupons");
                return Results.Ok(new ResponseDto<TResponse>
                {
                    Error = ex.Message,
                    IsSuccess = false
                });
            }
        }
    }
}
