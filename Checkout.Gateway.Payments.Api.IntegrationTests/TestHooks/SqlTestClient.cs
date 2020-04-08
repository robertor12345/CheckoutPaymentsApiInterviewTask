using Checkout.Gateway.Payments.Repositories.Entities;
using System.Data.SqlClient;
using Dapper.Contrib.Extensions;

namespace Checkout.Gateway.Payments.Api.IntegrationTests.TestHooks
{
    public class SqlTestClient
    {
        private readonly SqlConnection _databaseConnection;

        public SqlTestClient()
        {
            _databaseConnection =
                new SqlConnection(
                    "Server=(LocalDb)\\MyLocalDbInstance;Database=PaymentsDatabase;Trusted_Connection=True;");

            _databaseConnection.Open();
        }

    public void Open()
    {
    _databaseConnection.Open();
    }

    public async void SetupPaymentRecord(Payment payment)
    {
    const string query = @"SELECT *
                                    FROM [dbo].[Payment]";
     await _databaseConnection.InsertAsync(payment).ConfigureAwait(false);
    }
}
}
