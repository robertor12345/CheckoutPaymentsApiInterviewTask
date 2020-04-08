using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Entities;
using Checkout.Gateway.PaymentsApi.Infrastructure;
using Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions;
using Checkout.Gateway.PaymentsApi.Infrastructure.Logging;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Checkout.Gateway.PaymentsApi.Clients
{
    public class DatabaseClient : IDatabaseClient
    {
        private readonly SqlConnection _databaseConnection;

        public DatabaseClient()
        {
            _databaseConnection = new SqlConnection(Constants.SqlConnectionString);

            _databaseConnection.Open();
        }

        public async Task<int> InsertPaymentRecord(PaymentRequest paymentRequest, string paymentId, bool success)
        {
            try
            {
                return await _databaseConnection.InsertAsync(new Payment()
                {
                    SessionId = paymentRequest.MerchantSessionId,
                    PaymentId = paymentId,
                    TransactionStartTime = paymentRequest.TransactionStartTime,
                    CardNumber = paymentRequest.PaymentMethodDetails.CardNumber,
                    ExpiryMonth = paymentRequest.PaymentMethodDetails.ExpiryMonth,
                    ExpiryYear = paymentRequest.PaymentMethodDetails.ExpiryYear,
                    Cvv = paymentRequest.PaymentMethodDetails.Cvv,
                    PaymentAmount = paymentRequest.PaymentAmount,
                    Currency = paymentRequest.Currency,
                    Success = success.ToString()

                }).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw new SaveTransactionRecordException(MessageFactory.SaveTransactionRecordExceptionMessage);
            }
        }

        public void Open()
        {
            try
            {
                _databaseConnection.Open();
            }
            catch (Exception)
            {
                throw new SqlConnectionFailedException(MessageFactory.SqlConnectionFailureMessage);
            }
        }

        public async Task<Payment> QueryPaymentsBySessionIdAsync(string sessionId)
        {
            const string query = @"SELECT *
                                    FROM [dbo].[Payment]
                                    WHERE [SessionId] = @sessionId";
            var command = new CommandDefinition(query, new {sessionId});

            try
            {
                return await _databaseConnection
                    .QueryFirstOrDefaultAsync<Payment>(command).ConfigureAwait(false);
            }
            catch 
            {
                throw new QueryTransactionRecordFailedException(
                    MessageFactory.RetrieveTransactionRecordExceptionMessage);
            }
        }
    }
}