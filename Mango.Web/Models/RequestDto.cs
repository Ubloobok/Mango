namespace Mango.Web.Models
{
    public class RequestDto
    {
        public MethodType Method { get; set; } = MethodType.GET;
        public AuthorizationType Authorization { get; set; } = AuthorizationType.Bearer;
        public string Url { get; set; }
        public object Data { get; set; }
    }
}
