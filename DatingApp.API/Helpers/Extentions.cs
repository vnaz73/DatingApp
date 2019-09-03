using Microsoft.AspNetCore.Http;

namespace DatingApp.Api.Helpers
{
    public static class  Extentions
    {
        public static void AddApplicationError(this HttpResponse response,  string Message)
        {
            response.Headers.Add("Application-Error", Message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin","*");
        }
    }
}