using System.Net;

// Middleware to handle exceptions globally in the application
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next; // The next middleware in the pipeline
    private readonly ILogger<ExceptionHandlingMiddleware> _logger; // Logger for logging exceptions

    // Constructor to initialize the middleware with the next delegate and logger
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next; // Assign the next middleware
        _logger = logger; // Assign the logger
    }

    // Method to invoke the middleware
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            // Call the next middleware in the pipeline
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            // Log the exception details
            _logger.LogError(ex, "An unhandled exception has occurred.");
            // Handle the exception and return a response
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    // Method to handle the exception and return a response
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Set the content type of the response
        context.Response.ContentType = "application/json";

        // Set the default status code to Internal Server Error
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Get just the excetion message
        var result = new { message = exception.Message };

        // Write the response back to the client
        return context.Response.WriteAsJsonAsync(result);
    }
}
