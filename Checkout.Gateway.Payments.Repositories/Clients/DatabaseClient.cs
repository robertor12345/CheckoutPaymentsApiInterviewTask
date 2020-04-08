using System.Data.SqlClient;
using System.Threading.Tasks;
using Checkout.Gateway.Payments.Contracts;
using Checkout.Gateway.Payments.Repositories.Entities;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Checkout.Gateway.Payments.Repositories.Clients
{
    public class DatabaseClient : IDatabaseClient
    {
        private readonly SqlConnection _databaseConnection;

        public DatabaseClient()
        {
            _databaseConnection = new SqlConnection("Server=(LocalDb)\\MyLocalDbInstance;Database=PaymentsDatabase;Trusted_Connection=True;");

            _databaseConnection.Open();
        }

        public async Task<int> InsertPaymentRecord(PaymentRequest paymentRequest, string paymentId, bool success)
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

        public void Open()
        {
            _databaseConnection.Open();
        }

        public async Task<Payment> QueryPaymentsBySessionAsync(string sessionId)
        {
            const string query = @"SELECT *
                                    FROM [dbo].[Payment]";

            var command = new CommandDefinition(query, new {sessionId});
            return await _databaseConnection
                .QuerySingleOrDefaultAsync<Payment>(command).ConfigureAwait(false);
        }
    }
}