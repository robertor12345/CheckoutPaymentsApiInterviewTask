using System;
using System.Threading.Tasks;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Entities;
using Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions;
using Checkout.Gateway.PaymentsApi.Services;

namespace Checkout.Gateway.PaymentsApi.UnitTests.Fakes
{
    public class FakeGetPaymentService: IGetPaymentsService
    {
        private PaymentTransactionDetails _paymentTransactionDetails;

        public bool IsSuccessful;
        public Exception Exception;

        public void SuccessfullyReturns( PaymentTransactionDetails paymentTransactionDetails)
        {
            IsSuccessful = true;
           _paymentTransactionDetails = paymentTransactionDetails;

        }

        public void ThrowsUnhandledException()
        { 
            IsSuccessful = false;
            Exception = new Exception();
        }

        public void ThrowsPaymentNotFoundException()
        {
            IsSuccessful = false;
            Exception = new PaymentNotFoundException("A payment record for MerchantSessionId DummySessionID was not found, please ensure that this value is correct");
        }

        public Task<PaymentTransactionDetails> CreatePaymentDetailsResponse(string sessionId)
        {
            if (!IsSuccessful)
            {
                throw Exception;
            }
            else
            {
                return Task.FromResult(_paymentTransactionDetails);
            }
        }

        public Task<Payment> GetPaymentBySessionId(string sessionId)
        {
            throw new System.NotImplementedException();
        }
    }
}