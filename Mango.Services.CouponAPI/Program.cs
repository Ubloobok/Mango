using Mango.Services.CouponAPI;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Handlers;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.FormatterName = ConsoleFormatterNames.Simple;
});
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.ColorBehavior = LoggerColorBehavior.Enabled;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("Mango_Coupon"));
builder.Services.AddTransient<IHandler<GetFooArgs, Foo>, GetFooHandler>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
    dbContext.Coupons.AddRange(
        new Coupon { CouponId = 1, CouponCode = "TEST_ONE", DiscountAmount = 10 },
        new Coupon { CouponId = 2, CouponCode = "TEST_TWO", DiscountAmount = 20 }
    );
    dbContext.SaveChanges();
}

var fooApi = app.MapGroup("/api/foo");

fooApi.MapGet("versionOne/{want:bool}", async (bool request, ILogger<Program> logger, IHandler<GetFooArgs, Foo> handler) =>
{
    try
    {
        var @in = new GetFooArgs
        {
            WantFoo = request,
        };
        var @out = handler.Handle(@in);
        return Results.Ok(new ResponseDto<FooDto>
        {
            Result = @out.Map(),
            IsSuccess = true
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error fetching coupons");
        return Results.Ok(new ResponseDto<FooDto>
        {
            Error = ex.Message,
            IsSuccess = false
        });
    }
});

fooApi.MapGet("versionTwo/{want:bool}", async (
    bool request,
    HttpContext http,
    ILoggerFactory factory,
    IHandler<GetFooArgs, Foo> handler) =>
{
    return NoMediatrPipeline.Handle(factory, http, handler,
        request,
        (bool request) => new GetFooArgs { WantFoo = request },
        (Foo @out) => @out.Map());
});

fooApi.MapPost("versionTwo", async (
    [FromBody] GetFooRequest request, 
    HttpContext http,
    ILoggerFactory factory,
    IHandler<GetFooArgs, Foo> handler) =>
{
    return NoMediatrPipeline.Handle(factory, http, handler,
        request,
        (GetFooRequest request) => new GetFooArgs { WantFoo = request.WantFoo },
        (Foo @out) => @out.Map());
});

app.Run();
