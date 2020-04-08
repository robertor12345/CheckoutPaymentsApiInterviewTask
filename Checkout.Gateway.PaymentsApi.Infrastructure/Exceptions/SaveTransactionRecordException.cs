using System;

namespace Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions
{
    public class SaveTransactionRecordException : Exception
    {
        public SaveTransactionRecordException(string message) : base(message: message)
        {
        }
    }
}