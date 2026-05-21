namespace Courses.Application.Utilities;

public static class UrlValidator
{
    public static bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp ||
            uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
