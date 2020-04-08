using System.Threading.Tasks;
using Checkout.Gateway.Payments.Contracts;
using Checkout.Gateway.Payments.Repositories.Entities;

namespace Checkout.Gateway.Payments.Repositories.Clients
{
    public interface IDatabaseClient
    {
         Task<Payment> QueryPaymentsBySessionAsync(string sessionId);
         Task<int> InsertPaymentRecord(PaymentRequest paymentRequest, string paymentId, bool success);
         void Open();
    }
}