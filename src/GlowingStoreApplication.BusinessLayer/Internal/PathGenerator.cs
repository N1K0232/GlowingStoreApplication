namespace GlowingStoreApplication.BusinessLayer.Internal;

internal static class PathGenerator
{
    internal static string CreatePath(string fileName)
    {
        var now = DateTime.UtcNow;
        return Path.Combine(now.Year.ToString("0000"), now.Month.ToString("00"), now.Day.ToString("00"), fileName);
    }
}