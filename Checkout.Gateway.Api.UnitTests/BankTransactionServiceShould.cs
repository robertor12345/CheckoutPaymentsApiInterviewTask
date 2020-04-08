using System;
using System.Threading.Tasks;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Adapters;
using Checkout.Gateway.PaymentsApi.Clients;
using Checkout.Gateway.PaymentsApi.Infrastructure.Exceptions;
using Checkout.Gateway.PaymentsApi.Infrastructure.Logging;
using Checkout.Gateway.PaymentsApi.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace Checkout.Gateway.PaymentsApi.UnitTests
{
    public class BankTransactionServiceShould
    {
        private const string MerchantSessionId = "88c36b14-5381-40bf-be2a-a3ce75d6202b";
        private const string PaymentId = "88c36b14-5381-40bf-be2a-a3ce75d6202b";

        private const string CardNumber = "5555555555554444";
        private const int ExpiryMonth = 12;
        private const int ExpiryYear = 2030;
        private const int Cvv = 999;

        private  const double PaymentAmount = 999.99;
        private const string Currency = "GBP";

        [Test]
        public void ProcessBankTransaction_WhenProcessBankTransactionIsCalled()
        {
            const bool success = true;

            var expectedPaymentsResponse =
                new PaymentResponse(MerchantSessionId, PaymentId, success, PaymentAmount, Currency);

            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);


            var paymentRequest = new PaymentRequest(MerchantSessionId, PaymentId, transactionStartTime,
                paymentMethodDetails, PaymentAmount, Currency);


            var databaseClient = new Mock<IDatabaseClient>();

            databaseClient
                .Setup(n => n.InsertPaymentRecord(It.IsAny<PaymentRequest>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(1));

            var bankAdapter = new Mock<IBankAdapter>();

            bankAdapter.Setup(n => n.CallApi(paymentMethodDetails, Currency, PaymentAmount))
                .Returns(Task.FromResult(new BankResponse("", success)));

            var bankTransactionService =
                new BankTransactionService(new Mock<ILogger>().Object, databaseClient.Object, bankAdapter.Object);

            var result = bankTransactionService.ProcessBankTransaction(paymentRequest).Result;

            result.Should().BeEquivalentTo(expectedPaymentsResponse, options => options.Excluding(n => n.PaymentId));
        }

        [Test]
        public void CreateCorrectPaymentResponse_WhenCreatePaymentResponseIsCalled()
        {
            const bool success = true;

            var expectedPaymentsResponse =
                new PaymentResponse(MerchantSessionId, PaymentId, success, PaymentAmount, Currency);

            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            var paymentRequest = new PaymentRequest(MerchantSessionId, PaymentId, transactionStartTime,
                paymentMethodDetails, PaymentAmount, Currency);


            var databaseClient = new Mock<IDatabaseClient>();

            databaseClient
                .Setup(n => n.InsertPaymentRecord(It.IsAny<PaymentRequest>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(1));

            var bankAdapter = new Mock<IBankAdapter>();

            bankAdapter.Setup(n => n.CallApi(paymentMethodDetails, Currency, PaymentAmount))
                .Returns(Task.FromResult(new BankResponse("DummyTransactionId", success)));

            var bankTransactionService =
                new BankTransactionService(new Mock<ILogger>().Object, databaseClient.Object, bankAdapter.Object);

            var result = bankTransactionService.CreatePaymentResponse(new BankResponse("DummyTransactionId", true), paymentRequest)
                .Result;

            result.Should().BeEquivalentTo(expectedPaymentsResponse, options => options.Excluding(n => n.PaymentId));
        }

        [Test]
        public void SaveTransactionRecordToDatabaseWithStatus()
        {
            const int expectedRowsAffected = 1;

            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            const bool success = true;

            var paymentRequest = new PaymentRequest(MerchantSessionId, PaymentId, transactionStartTime,
                paymentMethodDetails, PaymentAmount, Currency);

            var databaseClient = new Mock<IDatabaseClient>();

            databaseClient
                .Setup(n => n.InsertPaymentRecord(It.IsAny<PaymentRequest>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(expectedRowsAffected));

            var bankAdapter = new Mock<IBankAdapter>();

            var bankResponse = new BankResponse("DummyTransactionId", success);
            bankAdapter.Setup(n => n.CallApi(paymentMethodDetails, Currency, PaymentAmount))
                .Returns(Task.FromResult(bankResponse));

            var logger = new Mock<ILogger>();

            var bankTransactionService =
                new BankTransactionService(logger.Object, databaseClient.Object, bankAdapter.Object);

            var response = bankTransactionService.SaveTransactionRecordWithStatus(bankResponse, paymentRequest).GetAwaiter().GetResult();

            response.Should().Be(expectedRowsAffected);
        }

        [Test]
        public void LogsStartMessage_WhenBankTransactionProcessIsStarted()
        {

            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            var paymentRequest = new PaymentRequest(MerchantSessionId, PaymentId, transactionStartTime,
                paymentMethodDetails, PaymentAmount, Currency);

            var logger = new Mock<ILogger>();

            var databaseClient = new Mock<IDatabaseClient>();

            databaseClient
                .Setup(n => n.InsertPaymentRecord(It.IsAny<PaymentRequest>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(1));

            var bankAdapter = new Mock<IBankAdapter>();

            bankAdapter.Setup(n => n.CallApi(paymentMethodDetails, Currency, PaymentAmount))
                .Returns(Task.FromResult(new BankResponse("DummyTransactionId", true)));

            var bankTransactionService =
                new BankTransactionService(logger.Object, databaseClient.Object, bankAdapter.Object);

            bankTransactionService.ProcessBankTransaction(paymentRequest).ConfigureAwait(false);

            logger.Verify(l => l.Information(MessageFactory.BankTransactionProcessStartedMessageTemplate, MerchantSessionId), Times.Once);
        }

        [Test]
        public void LogsCompletionMessage_WhenBankTransactionProcessIsCompleted()
        {

            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            var paymentRequest = new PaymentRequest(MerchantSessionId, PaymentId, transactionStartTime,
                paymentMethodDetails, PaymentAmount, Currency);

            var logger = new Mock<ILogger>();

            var databaseClient = new Mock<IDatabaseClient>();

            databaseClient
                .Setup(n => n.InsertPaymentRecord(It.IsAny<PaymentRequest>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(1));

            var bankAdapter = new Mock<IBankAdapter>();

            bankAdapter.Setup(n => n.CallApi(paymentMethodDetails, Currency, PaymentAmount))
                .Returns(Task.FromResult(new BankResponse("DummyTransactionId", true)));

            var bankTransactionService =
                new BankTransactionService(logger.Object, databaseClient.Object, bankAdapter.Object);

            bankTransactionService.ProcessBankTransaction(paymentRequest).ConfigureAwait(false);

            logger.Verify(l => l.Information(MessageFactory.BankTransactionProcessCompletedMessageTemplate, MerchantSessionId), Times.Once);

        }

        [Test]
        public void LogsError_WhenSaveTransactionRecordWithStatusFailsWithSaveTransactionRecordException()
        {
            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            const bool success = true;

            var paymentRequest = new PaymentRequest(MerchantSessionId, PaymentId, transactionStartTime,
                paymentMethodDetails, PaymentAmount, Currency);

            var databaseClient = new Mock<IDatabaseClient>();

            databaseClient
                .Setup(n => n.InsertPaymentRecord(It.IsAny<PaymentRequest>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Throws(new SaveTransactionRecordException( MessageFactory.SaveTransactionRecordExceptionMessage));

            var bankAdapter = new Mock<IBankAdapter>();

            var bankResponse = new BankResponse("DummyTransactionId", success);
            bankAdapter.Setup(n => n.CallApi(paymentMethodDetails, Currency, PaymentAmount))
                .Returns(Task.FromResult(bankResponse));

            var logger = new Mock<ILogger>();

            var bankTransactionService =
                new BankTransactionService(logger.Object, databaseClient.Object, bankAdapter.Object);

            bankTransactionService.ProcessBankTransaction(paymentRequest).ConfigureAwait(false);

            logger.Verify(l => l.Error(MessageFactory.SaveTransactionRecordFailedLogMessageTemplate, MerchantSessionId), Times.Once);
        }

        [Test]
        public void LogsError_WhenSaveTransactionRecordWithStatusFailsWithSqlConnectionFailedException()
        {
            var transactionStartTime = new DateTime(1, 1, 1, 1, 1, 1, 1);

            var paymentMethodDetails = new PaymentMethodDetails(CardNumber, ExpiryMonth, ExpiryYear, Cvv);

            const bool success = true;

            var paymentRequest = new PaymentRequest(MerchantSessionId, PaymentId, transactionStartTime,
                paymentMethodDetails, PaymentAmount, Currency);

            var databaseClient = new Mock<IDatabaseClient>();

            databaseClient
                .Setup(n => n.InsertPaymentRecord(It.IsAny<PaymentRequest>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Throws(new SqlConnectionFailedException(MessageFactory.SqlConnectionFailureMessage));

            var bankAdapter = new Mock<IBankAdapter>();

            var bankResponse = new BankResponse("DummyTransactionId", success);
            bankAdapter.Setup(n => n.CallApi(paymentMethodDetails, Currency, PaymentAmount))
                .Returns(Task.FromResult(bankResponse));

            var logger = new Mock<ILogger>();

            var bankTransactionService =
                new BankTransactionService(logger.Object, databaseClient.Object, bankAdapter.Object);

            bankTransactionService.ProcessBankTransaction(paymentRequest).ConfigureAwait(false);

            logger.Verify(l => l.Error(MessageFactory.SqlConnectionFailedLogMessageTemplate, MerchantSessionId), Times.Once);
        }
    }
}