using System;
using System.Threading.Tasks;
using Checkout.Gateway.Payments.Api.Services;
using Checkout.Gateway.Payments.Contracts;

namespace Checkout.Gateway.Payments.Api.UnitTests.Fakes
{
    public class FakeBankTransactionService : IBankTransactionService
    {
        private bool _isSuccessful;
        private PaymentResponse _paymentResponse;
        private readonly PaymentRequest _paymentRequest;

        public void SuccessfullyReturns(PaymentResponse paymentResponse)
        {
            _isSuccessful = true;
            _paymentResponse = paymentResponse;
        }

        public FakeBankTransactionService(PaymentRequest paymentRequest)
        {
            _paymentRequest = paymentRequest;
        }


        public Task<PaymentResponse> ProcessBankTransaction(PaymentRequest paymentRequest)
        {
            if (!_isSuccessful)
            {
                throw new Exception("Unable to contact bank system to process the transaction");
            }

            return Task.FromResult(_paymentResponse);

        }

        public Task<BankResponse> RequestBankTransaction()
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateTransactionStatus(BankResponse bankResponse)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentResponse> CreatePaymentResponse(BankResponse bankResponse)
        {
            throw new NotImplementedException();
        }
    }
}