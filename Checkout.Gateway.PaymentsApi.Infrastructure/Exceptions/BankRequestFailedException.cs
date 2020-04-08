using System;

namespace Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions
{
    public class BankRequestFailedException : Exception
    {
        public BankRequestFailedException(string message) : base(message: message)
        {

        }
    }
}