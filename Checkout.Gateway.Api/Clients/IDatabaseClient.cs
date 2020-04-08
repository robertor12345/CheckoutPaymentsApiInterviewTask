using System.Threading.Tasks;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Entities;

namespace Checkout.Gateway.PaymentsApi.Clients
{
    public interface IDatabaseClient
    {
        public Task<Payment> QueryPaymentsBySessionIdAsync(string sessionId);
        public Task<int> InsertPaymentRecord(PaymentRequest paymentRequest, string paymentId, bool success);
        public void Open();
    }
}