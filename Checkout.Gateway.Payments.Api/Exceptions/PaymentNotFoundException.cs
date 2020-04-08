using System;

namespace Checkout.Gateway.PaymentsApi.Exceptions
{
    public class PaymentNotFoundException: Exception
    {
        public PaymentNotFoundException(string message) : base(message: message) {
        }
    }
}