using System;

namespace Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions
{
    public class PaymentNotFoundException: Exception
    {
        public PaymentNotFoundException(string message) : base(message: message) {
        }
    }
}