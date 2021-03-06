using System;
using Dapper.Contrib.Extensions;

namespace Checkout.Gateway.Payments.Repositories.Entities
{
    [Table("[Payment]")]
    public class Payment
    { 
        [Key]
        public string SessionId { get; set; }
        public string PaymentId { get; set; }
        public DateTime TransactionStartTime { get; set; }
        public string CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public int Cvv { get; set; }
        public double PaymentAmount { get; set; }
        public string Currency { get; set; }
        public string Success { get; set; }
    }
}