using System;

namespace Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions
{
    public class PaymentRequestInvalidException : Exception
    {
        public PaymentRequestInvalidException(string message) : base(message: message)
        {
        }
    }
}