using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Adapters;
using Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions;
using Checkout.Gateway.PaymentsApi.Infrastructure.FakeBankApi;
using Checkout.Gateway.PaymentsApi.Infrastructure.Logging;
using NUnit.Framework;
using FluentAssertions;
using Serilog;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;

namespace Checkout.Gateway.PaymentsApi.UnitTests
{
    public class BankAdapterShould
    {
        private const string CardNumber = "5555555555554444";
        private const int ExpiryMonth = 12;
        private const int ExpiryYear = 2020;
        private const int Cvv = 999;

        private const double PaymentAmount = 999.99;
        private const string Currency = "GBP";

        [Test]
        public void ReturnExpectedBankResponse_WhenCallApiIsInvokedWithBankRequest()
        {
            const string transactionId = "00000000-0000-0000-0000-000000000000";

            const bool transactionStatus = true;

            var expectedBankResponse = new BankResponse(transactionId, transactionStatus);

            var httpMessageHandler = new MockHttpMessageHandler();

            var bankResponse = JsonConvert.SerializeObject(expectedBankResponse);

            httpMessageHandler.When("http://dummy-bank-url/api/validate-method")
                .Respond("application/json", bankResponse);

            var httpClient = new HttpClient(httpMessageHandler);

            var logger = new Mock<ILogger>();

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            var bankAdapter = new BankAdapter(logger.Object, new BankRequestClient(httpClient));

            var result = bankAdapter.CallApi(paymentMethodDetails, Currency, PaymentAmount).GetAwaiter().GetResult();

            result.Should().BeEquivalentTo(expectedBankResponse);
        }

        [Test]
        public void ThrowBankRequestFailedException_WhenBankRequestFails()
        {
            var bankRequestClient = new Mock<IBankRequestClient>();

            bankRequestClient.Setup(n => n.CallApi(It.IsAny<BankTransactionRequest>()))
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)));

            var logger = new Mock<ILogger>();

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            var bankAdapter = new BankAdapter(logger.Object, bankRequestClient.Object);

            Action action = () => bankAdapter.CallApi(paymentMethodDetails, Currency, PaymentAmount).GetAwaiter().GetResult();

            action.Should().Throw<BankRequestFailedException>(MessageFactory.ProcessPaymentFailedUnableToContactBankMessage);
        }


        [Test]
        public void LogStartMessage_WhenBankAdapterTransactionProcessBegins()
        {
            const string transactionId = "00000000-0000-0000-0000-000000000000";

            const bool transactionStatus = true;

            var expectedBankResponse = new BankResponse(transactionId, transactionStatus);

            var httpMessageHandler = new MockHttpMessageHandler();

            var bankResponse = JsonConvert.SerializeObject(expectedBankResponse);

            httpMessageHandler.When("http://dummy-bank-url/api/validate-method")
                .Respond("application/json", bankResponse);

            var httpClient = new HttpClient(httpMessageHandler);

            var logger = new Mock<ILogger>();

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);
            
            var bankAdapter = new BankAdapter(logger.Object, new BankRequestClient(httpClient));

            bankAdapter.CallApi(paymentMethodDetails, Currency, PaymentAmount).GetAwaiter().GetResult();

            logger.Verify(l => l.Information(MessageFactory.BankAdapterProcessStartedLogMessage), Times.Once);

        }


        [Test]
        public void LogCompleteMessage_WhenBankAdapterTransactionProcessBegins()
        {
            const string transactionId = "00000000-0000-0000-0000-000000000000";

            const bool transactionStatus = true;

            var expectedBankResponse = new BankResponse(transactionId, transactionStatus);

            var httpMessageHandler = new MockHttpMessageHandler();

            var bankResponse = JsonConvert.SerializeObject(expectedBankResponse);

            httpMessageHandler.When("http://dummy-bank-url/api/validate-method")
                .Respond("application/json", bankResponse);

            var httpClient = new HttpClient(httpMessageHandler);

            var logger = new Mock<ILogger>();

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            var bankAdapter = new BankAdapter(logger.Object, new BankRequestClient(httpClient));

            bankAdapter.CallApi(paymentMethodDetails, Currency, PaymentAmount).GetAwaiter().GetResult();

            logger.Verify(l => l.Information(MessageFactory.BankAdapterProcessCompletedLogMessage), Times.Once);

        }

        [Test]
        public void LogErrorMessage_WhenBankAdapterTransactionProcessFailsToContactBank()
        {
            var bankRequestClient = new Mock<IBankRequestClient>();

            bankRequestClient.Setup(n => n.CallApi(It.IsAny<BankTransactionRequest>()))
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)));

            var logger = new Mock<ILogger>();

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            var bankAdapter = new BankAdapter(logger.Object, bankRequestClient.Object);

            Action action = () => bankAdapter.CallApi(paymentMethodDetails, Currency, PaymentAmount).GetAwaiter().GetResult();

            action.Should().Throw<BankRequestFailedException>(MessageFactory.ProcessPaymentFailedUnableToContactBankMessage);

            logger.Verify(l => l.Error(MessageFactory.BankAdapterProcessFailedLogMessage), Times.Once);

        }

    }
}