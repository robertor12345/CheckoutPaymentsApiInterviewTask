using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Checkout.Gateway.Payments.Api.Adapters;
using Checkout.Gateway.Payments.Contracts;
using Checkout.Gateway.Payments.Repositories.Clients;
using Checkout.Gateway.PaymentsApi.Adapters;
using Checkout.Gateway.PaymentsApi.Logging;
using Serilog;

namespace Checkout.Gateway.Payments.Api.Services
{
    public class BankTransactionService : IBankTransactionService
    {
        private readonly ILogger _logger;
        private readonly IDatabaseClient _databaseClient;
        private PaymentRequest _paymentRequest;
        private ILogger logger;
        private DatabaseClient databaseClient;
        private BankAdapter bankAdapter;
        private readonly IBankAdapter _bankAdapter;
        private readonly string _paymentId;


        public BankTransactionService(ILogger logger, IDatabaseClient databaseClient, IBankAdapter bankAdapter)
        {
            _logger = logger;
            _databaseClient = databaseClient;
            _bankAdapter = bankAdapter;
            _paymentId = GeneratePaymentId();
        }

        public BankTransactionService(ILogger logger, DatabaseClient databaseClient, BankAdapter bankAdapter)
        {
            this.logger = logger;
            this.databaseClient = databaseClient;
            this.bankAdapter = bankAdapter;
        }

        public Task<PaymentResponse> ProcessBankTransaction(PaymentRequest paymentRequest)
        {
            _logger.Information(LogMessageFactory.BankTransactionProcessStartedMessageTemplate, _paymentRequest.MerchantSessionId);

            _paymentRequest = paymentRequest;

            var bankResponse = RequestBankTransaction().GetAwaiter().GetResult();

            UpdateTransactionStatus(bankResponse);

            _logger.Information(LogMessageFactory.BankTransactionProcessCompletedMessageTemplate, _paymentRequest.MerchantSessionId);
            return CreatePaymentResponse(bankResponse);
        }

        public Task<BankResponse> RequestBankTransaction()
        {
            return Task.FromResult(_bankAdapter.CallApi(_paymentRequest.PaymentMethodDetails, _paymentRequest.Currency, _paymentRequest.PaymentAmount).Result);
        }


        public Task<int> UpdateTransactionStatus(BankResponse bankResponse)
        {
            try
            {
                _logger.Information(LogMessageFactory.InsertPaymentRecordMessageTemplate, _paymentRequest.MerchantSessionId );
                return _databaseClient.InsertPaymentRecord(_paymentRequest, _paymentId, bankResponse.TransactionStatus);
            }
            catch (SqlException ex)
            {
                _logger.Error(ex, LogMessageFactory.InsertPaymentRecordFailedMessageTemplate, _paymentRequest.MerchantSessionId);
                throw ex;
            }
        }

        public Task<PaymentResponse> CreatePaymentResponse(BankResponse bankResponse)
        {
            return Task.FromResult(new PaymentResponse(_paymentRequest.MerchantSessionId, _paymentId,
                bankResponse.TransactionStatus, _paymentRequest.PaymentAmount, _paymentRequest.Currency));
        }

        private static string GeneratePaymentId()
        {
            return new Guid().ToString();
        }
    }
}