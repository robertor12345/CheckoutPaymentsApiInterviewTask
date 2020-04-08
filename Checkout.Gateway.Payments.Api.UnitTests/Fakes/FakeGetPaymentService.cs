using System;
using System.Threading.Tasks;
using Checkout.Gateway.Payments.Api.Services;
using Checkout.Gateway.Payments.Contracts;
using Checkout.Gateway.Payments.Repositories.Entities;

namespace Checkout.Gateway.Payments.Api.UnitTests.Fakes
{
    public class FakeGetPaymentService: IGetPaymentsService
    {
        private PaymentTransactionDetails _paymentTransactionDetails;

        public bool _isSuccessful;

        public void SuccessfullyReturns( PaymentTransactionDetails paymentTransactionDetails)
        {
            _isSuccessful = true;
           _paymentTransactionDetails = paymentTransactionDetails;

        }

        public void ThrowsException()
        { 
            _isSuccessful = false;;
        }

        public Task<PaymentTransactionDetails> CreatePaymentDetailsResponse(string sessionId)
        {
            if (!_isSuccessful == true)
            {
                throw new Exception("Payment Not Found");
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