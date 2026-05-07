namespace Courses.Application.Utilities;

public static class Helper
{
    public static int GetRightOrder(int lastOrder)
    {
        if (lastOrder == 0)
            return 10;
        else
            return ((lastOrder / 10) + 1) * 10; // should be match the pattern: 10, 20, 30, ...
    }
    public static bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp ||
            uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
