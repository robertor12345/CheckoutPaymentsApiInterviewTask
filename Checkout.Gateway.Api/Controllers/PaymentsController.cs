using System;
using System.Threading.Tasks;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions;
using Checkout.Gateway.PaymentsApi.Infrastructure.Logging;
using Checkout.Gateway.PaymentsApi.ResponseComposition;
using Checkout.Gateway.PaymentsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Checkout.Gateway.PaymentsApi.Controllers
{
    [ApiController]
    [Route("/gateway/[controller]/v1/")]
    public class PaymentsController : ControllerBase
    {
        private readonly ILogger _logger;

        private readonly IGetPaymentsService _getPaymentsService;

        private readonly IBankTransactionService _bankTransactionService;

        public PaymentsController(ILogger logger, IGetPaymentsService getPaymentService,
            IBankTransactionService bankTransactionService)
        {
            _logger = logger;
            _getPaymentsService = getPaymentService;
            _bankTransactionService = bankTransactionService;
        }

        [HttpGet("{merchantSessionId}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetPaymentDetails(string merchantSessionId)
        {
            try
            {
                var result = await _getPaymentsService.CreatePaymentDetailsResponse(merchantSessionId)
                    .ConfigureAwait(false);
                _logger.Information(MessageFactory.GetPaymentDetailsSuccessMessageTemplate, merchantSessionId);
                return Ok(result);
            }
            catch (PaymentNotFoundException)
            {
                _logger.Error(MessageFactory.GetPaymentDetailsFailedMessageTemplate, merchantSessionId);

                var friendlyErrorMessage = $"No Payment record was found for the sessionId {merchantSessionId}";

                const int statusCode = 404;
                var badRequestObjectResult = ResponseFactory.CreateBadRequestObjectResult(statusCode, friendlyErrorMessage);
                return badRequestObjectResult;
            }
            catch (Exception)
            {
                _logger.Error(MessageFactory.GetPaymentDetailsFailedWithUnhandledExceptionMessageTemplate,
                    merchantSessionId);

                var friendlyErrorMessage =
                    $"An Unhandled Exception has occured when attempting to retrieve payment for the Merchant Session ID {merchantSessionId}";
               
                const int statusCode = 500;
                var badRequestObjectResult = ResponseFactory.CreateBadRequestObjectResult(statusCode, friendlyErrorMessage);

                return badRequestObjectResult;
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest paymentRequest)
        {
            try
            {
                var result = await _bankTransactionService.ProcessBankTransaction(paymentRequest);

                if (!result.Success)
                {
                    _logger.Information(MessageFactory.PaymentProcessCompletedWithBankDeclineMessageTemplate, paymentRequest.MerchantSessionId);
                    return Ok(result);
                }

                _logger.Information(MessageFactory.PaymentProcessCompletedWithBankApprovalMessageTemplate,
                    paymentRequest.MerchantSessionId);
                return Ok(result);
            }
            catch (PaymentRequestInvalidException exception)
            {
                const int statusCode = 422;
                var badRequestObjectResult = ResponseFactory.CreateBadRequestObjectResult(statusCode, exception.Message);
                return badRequestObjectResult;
            }
            catch (BankRequestFailedException)
            {
                const int statusCode = 503;
                var badRequestObjectResult = ResponseFactory.CreateBadRequestObjectResult(statusCode, MessageFactory.ProcessPaymentFailedUnableToContactBankLogMessageTemplate);
                return badRequestObjectResult;
            }
        }

    }

}