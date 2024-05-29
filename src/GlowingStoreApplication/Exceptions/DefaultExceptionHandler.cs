using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GlowingStoreApplication.Exceptions;

public class DefaultExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService problemDetailsService;
    private readonly IWebHostEnvironment environment;

    public DefaultExceptionHandler(IProblemDetailsService problemDetailsService, IWebHostEnvironment environment)
    {
        this.problemDetailsService = problemDetailsService;
        this.environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = httpContext.Response.StatusCode,
            Title = exception.GetType().FullName,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        if (environment.IsDevelopment())
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        await problemDetailsService.WriteAsync(new()
        {
            HttpContext = httpContext,
            AdditionalMetadata = httpContext.Features.Get<IExceptionHandlerFeature>()?.Endpoint?.Metadata,
            ProblemDetails = problemDetails,
            Exception = exception
        });

        return true;
    }
}