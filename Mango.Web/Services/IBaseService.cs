using Mango.Web.Models;

namespace Mango.Web.Services
{
    public interface IBaseService
    {
        Task<ResponseDto<TResult>> SendAsync<TResult>(RequestDto requestDto);
    }
}
