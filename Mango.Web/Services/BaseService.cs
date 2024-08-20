using Mango.Web.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Mango.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDto<TResult>> SendAsync<TResult>(RequestDto requestDto)
        {
            var client = _httpClientFactory.CreateClient("MangoAPI");

            var apiMessage = new HttpRequestMessage
            {
                Method = new HttpMethod(requestDto.Method.ToString()),
                RequestUri = new Uri(requestDto.Url)
            };
            if (requestDto.Data != null)
            {
                var serializedData = JsonConvert.SerializeObject(requestDto.Data);
                apiMessage.Content = new StringContent(serializedData, Encoding.UTF8, "application/json");
            }

            apiMessage.Headers.Add("Accept", "application/json");
            if (requestDto.Authorization == AuthorizationType.Bearer)
            {
                var token = _tokenProvider.GetToken();
                apiMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
