using Mango.Web.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Mango.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseDto<TResult>> SendAsync<TResult>(RequestDto requestDto)
        {
            var client = _httpClientFactory.CreateClient("MangoAPI");

            var apiMessage = new HttpRequestMessage();
            apiMessage.Method = new HttpMethod(requestDto.ApiType.ToString());
            apiMessage.Headers.Add("Accept", "application/json");
            apiMessage.RequestUri = new Uri(requestDto.Url);
            if (requestDto.Data != null)
            {
                apiMessage.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json" );
            }

            var apiResponse = await client.SendAsync(apiMessage);

            if (apiResponse.StatusCode == HttpStatusCode.OK)
            {
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                var apiData = JsonConvert.DeserializeObject<ResponseDto<TResult>>(apiContent);
                return apiData;
            }
            else
            {
                return new ResponseDto<TResult>
                {
                    IsSuccess = false,
                    Error = apiResponse.ReasonPhrase
                };
            }
        }
    }
}
