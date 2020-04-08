using System.Threading.Tasks;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions;
using Checkout.Gateway.PaymentsApi.Infrastructure.FakeBankApi;
using Checkout.Gateway.PaymentsApi.Infrastructure.Logging;
using Newtonsoft.Json;
using Serilog;

namespace Checkout.Gateway.PaymentsApi.Adapters
{
    public class BankAdapter : IBankAdapter
    {
        private readonly IBankRequestClient _bankRequestClient;
        private readonly ILogger _logger;

        public BankAdapter(ILogger logger, IBankRequestClient bankRequestClient)
        {
            _logger = logger;
            _bankRequestClient = bankRequestClient;
        }

        public async Task<BankResponse> CallApi(PaymentMethodDetails paymentMethodDetails, string currency, double paymentAmount)
        {
            _logger.Information(MessageFactory.BankAdapterProcessStartedLogMessage);
            var bankTransactionRequest = CreateBankTransactionRequest(paymentMethodDetails, currency, paymentAmount);

            var response = await _bankRequestClient.CallApi(bankTransactionRequest).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.Error(MessageFactory.BankAdapterProcessFailedLogMessage);
                throw new BankRequestFailedException(MessageFactory.ProcessPaymentFailedUnableToContactBankMessage);
            }

            var responseString = response.Content.ReadAsStringAsync();

            var deserializedObject = JsonConvert.DeserializeObject<BankResponse>(responseString.Result);

            _logger.Information(MessageFactory.BankAdapterProcessCompletedLogMessage);

            return deserializedObject;
        }

        private static BankTransactionRequest CreateBankTransactionRequest(PaymentMethodDetails paymentMethodDetails, string currency, double paymentAmount)
        {
            return new BankTransactionRequest(paymentMethodDetails, currency, paymentAmount);
        }
    }
}