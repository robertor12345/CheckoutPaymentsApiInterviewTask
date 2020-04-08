using System.Threading.Tasks;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Clients;
using Checkout.Gateway.PaymentsApi.Entities;
using Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions;
using Checkout.Gateway.PaymentsApi.Infrastructure.Logging;
using Serilog;

namespace Checkout.Gateway.PaymentsApi.Services
{
    public class GetPaymentRecordService : IGetPaymentsService
    {
        private readonly ILogger _logger;
        private readonly IDatabaseClient _databaseClient;

        public GetPaymentRecordService(ILogger logger, IDatabaseClient databaseClient)
        {
            _logger = logger;
            _databaseClient = databaseClient;
        }

        public Task<PaymentTransactionDetails> CreatePaymentDetailsResponse(string sessionId)
        {
            _logger.Information(MessageFactory.GetPaymentRecordProcessStartedLogMessageTemplate, sessionId);

            var result = GetPaymentBySessionId(sessionId).Result;

            if (result == null)
            {
                _logger.Error(MessageFactory.GetPaymentRecordProcessSFailedLogMessageTemplate, sessionId);
                throw new PaymentNotFoundException(MessageFactory.GetPaymentRecordProcessSFailedMessage);
            }

            var response = new PaymentTransactionDetails(result.SessionId, result.PaymentId, result.TransactionStartTime,
                CreatePaymentMethodDetails(MaskLongCardNumber(result.CardNumber), result.ExpiryMonth, result.ExpiryYear, result.Cvv),
                result.PaymentAmount, result.Currency, result.Success);

            _logger.Information(MessageFactory.GetPaymentRecordProcessCompletedLogMessageTemplate, sessionId);
            return Task.FromResult(response);

        }

        private static PaymentMethodDetails CreatePaymentMethodDetails(string cardNumber, int expiryMonth, int expiryYear, int cvv)
        {
            return new PaymentMethodDetails(cardNumber, expiryMonth, expiryYear, cvv);
        }

        public async Task<Payment> GetPaymentBySessionId(string sessionId)
        {
            return await _databaseClient.QueryPaymentsBySessionIdAsync(sessionId);
        }

        public string MaskLongCardNumber(string longCardNumber)
        {
            return longCardNumber.Substring(longCardNumber.Length - 4);
        }
    }
}
