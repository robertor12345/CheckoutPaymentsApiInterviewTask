using System;
using System.Threading.Tasks;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions;
using Checkout.Gateway.PaymentsApi.Infrastructure.Logging;
using Checkout.Gateway.PaymentsApi.Services;

namespace Checkout.Gateway.PaymentsApi.UnitTests.Fakes
{
    public class FakeBankTransactionService : IBankTransactionService
    {
        private bool _isSuccessful;
        private bool _throwsPaymentRequestInvalidException;
        private PaymentResponse _paymentResponse;
        private readonly PaymentRequest _paymentRequest;
        private bool _throwsBankRequestFailedException;

        public void SuccessfullyReturns(PaymentResponse paymentResponse)
        {
            _isSuccessful = true;
            _paymentResponse = paymentResponse;
        }

        public void ThrowsBankRequestFailedException()
        {
            _isSuccessful = false;
            _throwsBankRequestFailedException = true;
        }

        public void ThrowsPaymentDetailsInvalidException()
        {
            _isSuccessful = false;
            _throwsPaymentRequestInvalidException = true;
        }


        public FakeBankTransactionService(PaymentRequest paymentRequest)
        {
            _paymentRequest = paymentRequest;
        }

        public Task<PaymentResponse> ProcessBankTransaction(PaymentRequest paymentRequest)
        {
            if (!_isSuccessful)
            {
                if (_throwsPaymentRequestInvalidException)
                {
                    throw new PaymentRequestInvalidException("Dummy message of payment validation failure type");
                }

                if (_throwsBankRequestFailedException)
                {
                    throw new BankRequestFailedException(MessageFactory.ProcessPaymentFailedUnableToContactBankMessage);
                }
            }
            return Task.FromResult(_paymentResponse);
        }

        public Task<BankResponse> RequestBankTransaction()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveTransactionRecordWithStatus(BankResponse bankResponse)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentResponse> CreatePaymentResponse(BankResponse bankResponse)
        {
            throw new NotImplementedException();
        }
    }
}