namespace Pi.BackofficeService.Application.Models
{

    [AttributeUsage(AttributeTargets.Field)]
    class ApiPathAttribute : Attribute
    {
        public string ApiPath;
        public ApiPathAttribute(string apiPath) { ApiPath = apiPath; }

    }
}