using System.Text;

namespace ImeWlConverter.Core.Helpers;

public static class HttpHelper
{
    public static string GetHtml(string url)
    {
        return GetHtml(url, Encoding.UTF8);
    }

    public static string GetHtml(string url, Encoding encoding)
    {
        var client = new HttpClient();
        var resp = client.GetStreamAsync(url).GetAwaiter().GetResult();
        return new StreamReader(resp, encoding).ReadToEnd();
    }
}
