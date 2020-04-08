using System.Net;
using Checkout.Gateway.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Checkout.Gateway.PaymentsApi.ResponseComposition
{
    public static class ResponseFactory
    {
        internal static BadRequestObjectResult CreateBadRequestObjectResult(int statusCode, string friendlyErrorMessage)
        {
            var badRequestObjectResult = new BadRequestObjectResult(new ErrorDetails()
                {
                    StatusCode = (HttpStatusCode)statusCode,
                    FriendlyErrorMessage = friendlyErrorMessage
                })
                { StatusCode = statusCode };
            return badRequestObjectResult;
        }
    }
}
