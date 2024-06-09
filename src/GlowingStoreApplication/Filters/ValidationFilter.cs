using System.Diagnostics;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace GlowingStoreApplication.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> validator;

    public ValidationFilter(IValidator<T> validator)
    {
        this.validator = validator;
    }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var input = context.Arguments.FirstOrDefault(a => a.GetType() == typeof(T)) as T;
        if (input is null)
        {
            return TypedResults.BadRequest();
        }

        var validationResult = await validator.ValidateAsync(input);
        if (validationResult.IsValid)
        {
            return await next.Invoke(context);
        }

        var httpContext = context.HttpContext;
        var statusCode = StatusCodes.Status400BadRequest;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Type = $"https://httpstatuses.io/{statusCode}",
            Title = "One or more validation errors occurred",
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        problemDetails.Extensions["errors"] = validationResult.ToDictionary();

        return TypedResults.Json(problemDetails, statusCode: statusCode, contentType: "application/problem+json; charset=utf-8");
    }
}