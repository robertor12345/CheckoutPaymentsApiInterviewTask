using System.Data.SqlClient;
using System.Threading.Tasks;
using Checkout.Gateway.PaymentsApi.Entities;
using Checkout.Gateway.PaymentsApi.Infrastructure;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Checkout.Gateway.PaymentsApi.IntegrationTests.TestHooks
{
    public class SqlTestClient
    {
        private readonly SqlConnection _databaseConnection;

        public SqlTestClient()
        {
            _databaseConnection =
                new SqlConnection(Constants.SqlConnectionString);
            _databaseConnection.Open();
        }

        public async Task SetupPaymentRecord(Payment payment)
        {
            await _databaseConnection.InsertAsync(payment).ConfigureAwait(false);
        }

        public async Task DeleteRecords()
        {
            const string sqlStatement = "DELETE FROM [dbo].Payment";
            await _databaseConnection.ExecuteAsync(sqlStatement).ConfigureAwait(false);
        }

        public async Task<Payment> GetPaymentRecord()
        { 
            const string sqlStatement = "SELECT * FROM [dbo].Payment";
           return await _databaseConnection.QueryFirstOrDefaultAsync<Payment>(sqlStatement).ConfigureAwait(false);
        }
    }
}