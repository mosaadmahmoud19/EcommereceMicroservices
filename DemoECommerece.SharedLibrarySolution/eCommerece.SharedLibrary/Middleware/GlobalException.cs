

using eCommerece.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace eCommerece.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {

        public async Task InvokeAsync(HttpContext context)
        {
            

        // Declare Default Variables

        string message = "sorry, internal server error occured. Kindely try again";

        int statusCode = (int)HttpStatusCode.InternalServerError;

        string title = "Error";

            try
            {
                await next(context);

                //check if Exception is too many request // 429 status code.

                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "Too many request made.";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, title, message, statusCode);
                }

                // if Respose is Unauthorized // 401 status code

                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    message = "You are not authorized to access.";
                    statusCode = (int)StatusCodes.Status401Unauthorized;
                    await ModifyHeader(context, title, message, statusCode);
                }

                // if Respose is Forbidden // 403 status code


                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out Of Access";
                    message = "You are not allowed / required to access.";
                    statusCode = (int)StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, message, statusCode);

                }
            }
            catch (Exception ex) 
            {
                // log Original Exceptions/ File , Debugger, Console

                LogException.LogExceptions(ex);

                // check if Execption is Timout // 408 request timeout

                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "out of time";
                    message = "Request timeout... try again";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }
                //if none of exceptions then do the default
                await ModifyHeader(context, title, message, statusCode);
            }




        }

        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            // display scary-free message to client

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails() 
            {
             Detail = message,
             Status = statusCode,
             Title = title
            
            }),CancellationToken.None);
            return;
        }
    }
}
