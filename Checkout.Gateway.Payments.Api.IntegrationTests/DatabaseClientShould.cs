using System;
using Checkout.Gateway.Payments.Api.IntegrationTests.TestHooks;
using Checkout.Gateway.Payments.Repositories.Clients;
using Checkout.Gateway.Payments.Repositories.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Checkout.Gateway.Payments.Api.IntegrationTests
{
    [TestFixture]
    public class DatabaseClientShould
    {

        private  SqlTestClient _sqlTestClient;

        [SetUp]
        public void Setup()
        {
            _sqlTestClient = new SqlTestClient();
        }


        [Test]
        public void ReturnCorrectPaymentRecordFromDatabase()
        {
            const string merchantSessionId = "DummySessionId";

            const string paymentId = "DummyPaymentId";

            var transactionStartTime = new DateTime(1,1,1,1,1,1, 1);

            const string cardNumber = "5555555555554444";
            const int expiryMonth = 12;
            const int expiryYear = 2020;
            const int cvv = 999;

            const double paymentAmount = 999.99;

            const string currency = "GBP";

            const string success = "true";

            var expectedResponse = new Payment
            {
                SessionId = merchantSessionId,
                PaymentId = paymentId,
                TransactionStartTime = transactionStartTime,
                CardNumber = cardNumber,
                ExpiryMonth = expiryMonth,
                ExpiryYear = expiryYear,
                Cvv = cvv,
                PaymentAmount = paymentAmount,
                Currency = currency,
                Success = success
            };

            _sqlTestClient.SetupPaymentRecord(expectedResponse);

            var dbClient = new DatabaseClient();

            var response = dbClient.QueryPaymentsBySessionAsync(merchantSessionId).GetAwaiter().GetResult();

            response.Should().BeOfType<Payment>();

            response.Should().BeEquivalentTo(expectedResponse);


        }
    }
}