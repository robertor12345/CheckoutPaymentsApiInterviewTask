using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Checkout.Gateway.Payments.Api.Services;
using Checkout.Gateway.Payments.Contracts;
using Moq;
using Checkout.Gateway.PaymentsApi.Adapters;
using Checkout.Gateway.Payments.Repositories.Clients;
using FluentAssertions;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Checkout.Gateway.Payments.Api.UnitTests
{
    public class BankTransactionServiceShould
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ProcessBankTransaction()
        {
            const string merchantSessionId = "DummySessionId";

            const string paymentId = "00000000-0000-0000-0000-000000000000";

            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            const string cardNumber = "5555555555554444";
            const int expiryMonth = 12;
            const int expiryYear = 2020;
            const int cvv = 999;

            var paymentMethodDetails = new PaymentMethodDetails(cardNumber, expiryMonth, expiryYear, cvv);

            const double paymentAmount = 999.99;

            const string currency = "GBP";

            const bool success = true;

            var paymentRequest = new PaymentRequest(merchantSessionId, paymentId, transactionStartTime,
                paymentMethodDetails, paymentAmount, currency);

            var expectedPaymentsResponse =
                new PaymentResponse(merchantSessionId, paymentId, success, paymentAmount, currency);

            var databaseClient = new Mock<IDatabaseClient>();

            databaseClient
                .Setup(n => n.InsertPaymentRecord(It.IsAny<PaymentRequest>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(1));

            var bankAdapter = new Mock<IBankAdapter>();

            bankAdapter.Setup(n => n.CallApi(paymentMethodDetails, currency, paymentAmount))
                .Returns(Task.FromResult(new BankResponse("", true)));

            var bankTransactionService =
                new BankTransactionService(new Mock<ILogger>().Object, databaseClient.Object, bankAdapter.Object);

            var result = bankTransactionService.ProcessBankTransaction(paymentRequest).Result;

            result.Should().BeEquivalentTo(expectedPaymentsResponse);
        }

        [Test]
        public void CreatePaymentResponse()
        {
            const string merchantSessionId = "DummySessionId";

            const string paymentId = "00000000-0000-0000-0000-000000000000";

            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            const string cardNumber = "5555555555554444";
            const int expiryMonth = 12;
            const int expiryYear = 2020;
            const int cvv = 999;

            var paymentMethodDetails = new PaymentMethodDetails(cardNumber, expiryMonth, expiryYear, cvv);

            const double paymentAmount = 999.99;

            const string currency = "GBP";

            const bool success = true;

            var paymentRequest = new PaymentRequest(merchantSessionId, paymentId, transactionStartTime,
                paymentMethodDetails, paymentAmount, currency);

            var expectedPaymentsResponse =
                new PaymentResponse(merchantSessionId, paymentId, success, paymentAmount, currency);

            var databaseClient = new Mock<IDatabaseClient>();

            databaseClient
                .Setup(n => n.InsertPaymentRecord(It.IsAny<PaymentRequest>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(1));

            var bankAdapter = new Mock<IBankAdapter>();

            bankAdapter.Setup(n => n.CallApi(paymentMethodDetails, currency, paymentAmount))
                .Returns(Task.FromResult(new BankResponse("", true)));

            var bankTransactionService =
                new BankTransactionService(new Mock<ILogger>().Object, databaseClient.Object, bankAdapter.Object);

            bankTransactionService.ProcessBankTransaction(paymentRequest);

            var result = bankTransactionService.CreatePaymentResponse(new BankResponse("DummyTransactionId", true))
                .Result;

            result.Should().BeEquivalentTo(expectedPaymentsResponse);
        }
    }
}