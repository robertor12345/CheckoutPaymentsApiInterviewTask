using System.Threading.Tasks;
using Checkout.Gateway.Payments.Contracts;
using Checkout.Gateway.Payments.Repositories.Clients;
using Checkout.Gateway.Payments.Repositories.Entities;
using Checkout.Gateway.PaymentsApi.Exceptions;
using Serilog;

namespace Checkout.Gateway.Payments.Api.Services
{
    public class GetPaymentService : IGetPaymentsService
    {
        private readonly ILogger _logger;
        private readonly IDatabaseClient _databaseClient;

        public GetPaymentService(ILogger logger, IDatabaseClient databaseClient)
        {
            _logger = logger;
            _databaseClient = databaseClient;
        }

        public Task<PaymentTransactionDetails> CreatePaymentDetailsResponse(string sessionId)
        {
            var result = GetPaymentBySessionId(sessionId).Result;

            if (result == null)
            {
                throw new PaymentNotFoundException($"A payment record for MerchantSessionId {sessionId} was not found, please ensure that this value is correct");
            }

            var response = new PaymentTransactionDetails(result.SessionId, result.PaymentId, result.TransactionStartTime,
                CreatePaymentMethodDetails(result.CardNumber, result.ExpiryMonth, result.ExpiryYear, result.Cvv),
                result.PaymentAmount, result.Currency, result.Success);

            return Task.FromResult(response);

        }

        private PaymentMethodDetails CreatePaymentMethodDetails(string cardNumber, int expiryMonth, int expiryYear, int cvv)
        {
            return new PaymentMethodDetails(cardNumber, expiryMonth, expiryYear, cvv);
        }

        public async Task<Payment> GetPaymentBySessionId(string sessionId)
        {
            return await _databaseClient.QueryPaymentsBySessionAsync(sessionId);
        }
    }
}
