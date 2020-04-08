using System;
using System.Threading.Tasks;
using Checkout.Gateway.Payments.Api.Adapters;
using Checkout.Gateway.Payments.Api.Services;
using Checkout.Gateway.Payments.Contracts;
using Checkout.Gateway.Payments.Repositories.Clients;
using Checkout.Gateway.PaymentsApi.Exceptions;
using Checkout.Gateway.PaymentsApi.Logging;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace Checkout.Gateway.Payments.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class PaymentsController : ControllerBase
    {

        private readonly ILogger _logger;

        private readonly IGetPaymentsService _getPaymentsService;

        private readonly IBankTransactionService _bankTransactionService;


        public PaymentsController(ILogger logger, IGetPaymentsService getPaymentService, IBankTransactionService bankTransactionService)
        {
            _logger = logger;

            if (getPaymentService != null)
            {
                _getPaymentsService = getPaymentService;
            }
            else
            {
                _getPaymentsService = new GetPaymentService(_logger, new DatabaseClient());
            }

            if (bankTransactionService != null)
            {
                _bankTransactionService = bankTransactionService;
            }
            else
            {
                _bankTransactionService = new BankTransactionService(_logger, new DatabaseClient(), new BankAdapter());
            }

        }

        [Microsoft.AspNetCore.Mvc.HttpGet("{merchantSessionId}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetPaymentDetails(string merchantSessionId)
        {
            try
            {
                var result = await _getPaymentsService.CreatePaymentDetailsResponse(merchantSessionId)
                    .ConfigureAwait(false);
                _logger.Information(LogMessageFactory.GetPaymentDetailsSuccessMessageTemplate, merchantSessionId);
                return Ok(result);
            }
            catch (PaymentNotFoundException ex)
            {
                _logger.Error(ex, LogMessageFactory.GetPaymentDetailsFailedMessageTemplate, merchantSessionId);
                return BadRequest($"No Payment record was found for the sessionId {merchantSessionId}");
            }
            catch (Exception)
            {
                _logger.Error(LogMessageFactory.GetPaymentDetailsFailedWithUnhandledExceptionMessageTemplate, merchantSessionId);
                return BadRequest($"Payment Failed - an Unhandled Exception has occured when attempting to retrieve payment for the Merchant Session ID {merchantSessionId}");
            }

        }

        public async Task<IActionResult> ProcessPayment(PaymentRequest paymentRequest)
        {
          
           try
           {
               var result = await _bankTransactionService.ProcessBankTransaction( paymentRequest);

               if (result.Success.Equals("false"))
               {
                   _logger.Error(LogMessageFactory.PaymentProcessFailedMessageTemplate, paymentRequest.MerchantSessionId);
                   return BadRequest(result);
               }

               _logger.Information(LogMessageFactory.PaymentProcessSuccessMessageTemplate, paymentRequest.MerchantSessionId);
               return Ok(result);
           }
           catch 
           {
               return BadRequest("Unable to process payment. An unhandled exception has occured");
           }
        }
    }
}