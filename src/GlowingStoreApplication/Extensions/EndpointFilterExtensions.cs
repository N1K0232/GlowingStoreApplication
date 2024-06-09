using GlowingStoreApplication.Filters;

namespace GlowingStoreApplication.Extensions;

public static class EndpointFilterExtensions
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
        => builder.AddEndpointFilter<ValidationFilter<T>>();
}