namespace Trecco.Application.Common.Results;

public static class ApiResults
{
    private sealed record ErrorMapping(string Title, string Detail, string Uri, int StatusCode);

    private static readonly Dictionary<ErrorType, ErrorMapping> _errorMappings = new()
    {
        [ErrorType.Validation] = new ErrorMapping(
            Title: "Validation failure",
            Detail: "One or more validation errors occurred.",
            StatusCode: StatusCodes.Status400BadRequest,
            Uri: "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        ),
        [ErrorType.Problem] = new ErrorMapping(
            Title: "Problem",
            Detail: "An unexpected problem occurred.",
            StatusCode: StatusCodes.Status400BadRequest,
            Uri: "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        ),
        [ErrorType.NotFound] = new ErrorMapping(
            Title: "Not Found",
            Detail: "The requested resource was not found.",
            StatusCode: StatusCodes.Status404NotFound,
            Uri: "https://tools.ietf.org/html/rfc7231#section-6.5.4"
        ),
        [ErrorType.Conflict] = new ErrorMapping(
            Title: "Conflict",
            Detail: "The request could not be completed due to a conflict.",
            StatusCode: StatusCodes.Status409Conflict,
            Uri: "https://tools.ietf.org/html/rfc7231#section-6.5.8"
        ),
        [ErrorType.Failure] = new ErrorMapping(
            Title: "Server failure",
            Detail: "An unexpected error occurred",
            StatusCode: StatusCodes.Status500InternalServerError,
            Uri: "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        )
    };

    public static IResult Problem(ResultBase result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot create problem result from successful operation");
        }

        Error error = result.Error;
        ErrorMapping mapping = _errorMappings.GetValueOrDefault(error.Type, _errorMappings[ErrorType.Failure]);

        return Microsoft.AspNetCore.Http.Results.Problem(
            title: mapping.Title,
            detail: mapping.Detail,
            statusCode: mapping.StatusCode,
            type: mapping.Uri,
            extensions: GetErrorExtensions(result)
        );
    }

    private static Dictionary<string, object?>? GetErrorExtensions(ResultBase result)
    {
        if (result.Error is not ValidationError validationError)
        {
            return null;
        }

        return new Dictionary<string, object?>
        {
            ["errors"] = validationError.Errors
        };
    }
}
