using System;
using System.Threading.Tasks;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Adapters;
using Checkout.Gateway.PaymentsApi.Clients;
using Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions;
using Checkout.Gateway.PaymentsApi.Infrastructure.Logging;
using Checkout.Gateway.PaymentsApi.Validators;
using Serilog;

namespace Checkout.Gateway.PaymentsApi.Services
{
    public class BankTransactionService : IBankTransactionService
    {
        private readonly ILogger _logger;
        private readonly IDatabaseClient _databaseClient;
        private readonly IBankAdapter _bankAdapter;
        private string _paymentId;

        public BankTransactionService(ILogger logger, IDatabaseClient databaseClient, IBankAdapter bankAdapter)
        {
            _logger = logger;
            _databaseClient = databaseClient;
            _bankAdapter = bankAdapter;
        }
        public Task<PaymentResponse> ProcessBankTransaction(PaymentRequest paymentRequest)
        {
            _logger.Information(MessageFactory.BankTransactionProcessStartedMessageTemplate,
                paymentRequest.MerchantSessionId);

            _paymentId = GeneratePaymentId();

            PaymentRequestValidator.ValidatePaymentRequestInformation(paymentRequest);

            var bankResponse = RequestBankTransaction(paymentRequest).GetAwaiter().GetResult();

            try
            {
                SaveTransactionRecordWithStatus(bankResponse, paymentRequest);
            }
            catch (SaveTransactionRecordException)
            {
                _logger.Error(MessageFactory.SaveTransactionRecordFailedLogMessageTemplate,
                    paymentRequest.MerchantSessionId);
                return CreatePaymentResponse(bankResponse, paymentRequest);
            }
            catch (SqlConnectionFailedException)
            {
                _logger.Error(MessageFactory.SqlConnectionFailedLogMessageTemplate, paymentRequest.MerchantSessionId);
                return CreatePaymentResponse(bankResponse, paymentRequest);
            }

            _logger.Information(MessageFactory.BankTransactionProcessCompletedMessageTemplate,
                paymentRequest.MerchantSessionId);
            return CreatePaymentResponse(bankResponse, paymentRequest);
        }

        public Task<BankResponse> RequestBankTransaction(PaymentRequest paymentRequest)
        {
            return _bankAdapter.CallApi(paymentRequest.PaymentMethodDetails, paymentRequest.Currency,
                paymentRequest.PaymentAmount);
        }

        public Task<int> SaveTransactionRecordWithStatus(BankResponse bankResponse, PaymentRequest paymentRequest)
        {
            _logger.Information(MessageFactory.InsertPaymentRecordMessageTemplate, paymentRequest.MerchantSessionId);
            return _databaseClient.InsertPaymentRecord(paymentRequest, _paymentId, bankResponse.TransactionStatus);
        }

        public Task<PaymentResponse> CreatePaymentResponse(BankResponse bankResponse, PaymentRequest paymentRequest)
        {
            return Task.FromResult(new PaymentResponse(paymentRequest.MerchantSessionId, _paymentId,
                bankResponse.TransactionStatus, paymentRequest.PaymentAmount, paymentRequest.Currency));
        }

        private static string GeneratePaymentId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
