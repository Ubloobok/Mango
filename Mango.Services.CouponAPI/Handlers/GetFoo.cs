using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models.Dto;
using Mango.Services.CouponAPI.Models;

namespace Mango.Services.CouponAPI.Handlers
{
    public class Foo
    {
        public int Id { get; set; }
    }

    public class GetFooRequest
    {
        public bool WantFoo { get; set; }
    }


    public class FooDto
    {
        public int Id { get; set; }
    }

    public static class FooDtoExtensions
    {
        public static FooDto Map(this Foo source) => new()
        {
            Id = source.Id
        };

        public static Foo? Map(this FooDto source) => source == null ? null : new()
        {
            Id = source.Id
        };
    }

    public class GetFooArgs
    {
        public bool WantFoo { get; set; }
    }

    public class GetFooHandler : IHandler<GetFooArgs, Foo>
    {
        private readonly ILogger _logger;
        private readonly AppDbContext _db;

        public GetFooHandler(ILogger<GetFooHandler> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public Foo Handle(GetFooArgs input)
        {
            _logger.LogInformation("Input {0}", input.WantFoo);
            if (input.WantFoo)
            {
                return new Foo { Id = 123 };
            }
            else
            {
                return new Foo { Id = 456 };
            }
        }
    }
}
