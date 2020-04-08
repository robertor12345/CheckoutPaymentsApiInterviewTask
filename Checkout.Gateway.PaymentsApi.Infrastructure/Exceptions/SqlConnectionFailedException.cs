using System;

namespace Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions
{
    public class SqlConnectionFailedException : Exception
    {
        public SqlConnectionFailedException(string message) :base(message: message)
        {
        }
    }
}