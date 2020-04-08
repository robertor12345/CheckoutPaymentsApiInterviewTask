using System;

namespace Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions
{
    public class QueryTransactionRecordFailedException : Exception
    {
        public QueryTransactionRecordFailedException(string message) :base(message: message)
        {
        }
    }
}