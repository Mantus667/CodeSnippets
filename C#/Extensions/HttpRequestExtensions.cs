using System.Linq;
using System.Web;
using System.Web.Hosting;

public static class HttpRequestExtensions
{
    public static string GetIpAddress(this HttpRequest request)
    {
        if (HostingEnvironment.IsDevelopmentEnvironment)
        {
            string devIpAddress = "XXX.XXX.XXX.XXX"; // Your IP address
            return devIpAddress;
        }

        string ipAddress =
            request
                ?.ServerVariables["HTTP_X_FORWARDED_FOR"]
                ?.Split(',')
                .ToList()
                .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            ipAddress = request?.ServerVariables["REMOTE_ADDR"];
        }

        return ipAddress;
    }
}
