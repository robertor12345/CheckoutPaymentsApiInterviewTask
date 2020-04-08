using System;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Clients;
using Checkout.Gateway.PaymentsApi.Entities;
using Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions;
using Checkout.Gateway.PaymentsApi.Infrastructure.Logging;
using Checkout.Gateway.PaymentsApi.IntegrationTests.TestHooks;
using FluentAssertions;
using NUnit.Framework;

namespace Checkout.Gateway.PaymentsApi.IntegrationTests
{
    public class DatabaseClientShould
    {
        private SqlTestClient _sqlTestClient;

        private const string MerchantSessionId = "DummySessionId";
        private const string PaymentId = "DummyPaymentId";

        private const string CardNumber = "5555555555554444";
        private const int ExpiryMonth = 12;
        private const int ExpiryYear = 2020;
        private const int Cvv = 999;

        private const double PaymentAmount = 999.99;
        private const string Currency = "GBP";
        private const bool Success = true;

        [SetUp]
        public void Setup()
        {
            _sqlTestClient = new SqlTestClient();
            _sqlTestClient.DeleteRecords().GetAwaiter().GetResult();
        }

        [TearDown]
        public void TearDown()
        {
            _sqlTestClient.DeleteRecords().GetAwaiter().GetResult();
        }

        [Test]
        public void ReturnCorrectPaymentRecordFromDatabase_WhenQueryPaymentsBySessionIdIsCalled()
        {
            var expectedResponse = new Payment
            {
                SessionId = MerchantSessionId,
                PaymentId = PaymentId,
                TransactionStartTime = new DateTime(9999, 12, 1, 1, 1, 1),
                CardNumber = CardNumber,
                ExpiryMonth = ExpiryMonth,
                ExpiryYear = ExpiryYear,
                Cvv = Cvv,
                PaymentAmount = PaymentAmount,
                Currency = Currency,
                Success = Success.ToString()
            };

            _sqlTestClient.SetupPaymentRecord(expectedResponse).GetAwaiter().GetResult();

            var dbClient = new DatabaseClient( );

            var response = dbClient.QueryPaymentsBySessionIdAsync(MerchantSessionId).GetAwaiter().GetResult();

            response.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public void InsertCorrectPaymentRecordFromDatabase_WhenInsertPaymentRecordIsCalled()
        {

            var expectedResponse = new Payment
            {
                SessionId = MerchantSessionId,
                PaymentId = PaymentId,
                TransactionStartTime = new DateTime(9999, 12, 1, 1, 1, 1),
                CardNumber = CardNumber,
                ExpiryMonth = ExpiryMonth,
                ExpiryYear = ExpiryYear,
                Cvv = Cvv,
                PaymentAmount = PaymentAmount,
                Currency = Currency,
                Success = Success.ToString()
            };

            var paymentRequest = new PaymentRequest(MerchantSessionId, PaymentId, new DateTime(9999, 12, 1, 1, 1, 1),
                new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv), PaymentAmount, Currency);

            var dbClient = new DatabaseClient();

            dbClient.InsertPaymentRecord(paymentRequest, PaymentId, Success).GetAwaiter().GetResult();

            var  response = _sqlTestClient.GetPaymentRecord().GetAwaiter().GetResult();

            response.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public void ThrowsSaveTransactionRecordException_WhenInsertPaymentRecordIsCalledAndSQLQueryFails()
        {
            var invalidPaymentRequest = new PaymentRequest(MerchantSessionId, PaymentId, new DateTime(1, 12, 1, 1, 1, 1, 1),
                new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv), PaymentAmount, Currency);

            var dbClient = new DatabaseClient();
 
            Action action = () => dbClient.InsertPaymentRecord(invalidPaymentRequest, PaymentId, Success).GetAwaiter().GetResult();

            action.Should().Throw<SaveTransactionRecordException>(MessageFactory.SaveTransactionRecordExceptionMessage);
        }
    }
}