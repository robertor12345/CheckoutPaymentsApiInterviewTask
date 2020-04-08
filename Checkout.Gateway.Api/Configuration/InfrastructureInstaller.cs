using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Checkout.Gateway.Api.Contracts;
using Checkout.Gateway.PaymentsApi.Adapters;
using Checkout.Gateway.PaymentsApi.Clients;
using Checkout.Gateway.PaymentsApi.Infrastructure.FakeBankApi;
using Checkout.Gateway.PaymentsApi.Services;
using Lamar;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Serilog;
using DatabaseClient = Checkout.Gateway.PaymentsApi.Clients.DatabaseClient;

namespace Checkout.Gateway.PaymentsApi.Configuration

{
    public class InfrastructureInstaller: ServiceRegistry
    {
        public InfrastructureInstaller()
        {
            For<ILogger>().Use(container => CreateLogger()).Singleton();

            For<IDatabaseClient>().Use<DatabaseClient>();
            For<IBankAdapter>().Use<BankAdapter>();

            For<IGetPaymentsService>().Use(container =>
                new GetPaymentRecordService(container.GetInstance<ILogger>(),
                    container.GetInstance<IDatabaseClient>()));

            For<IBankTransactionService>().Use(container => new BankTransactionService(container.GetInstance<ILogger>(),
                container.GetInstance<IDatabaseClient>(), container.GetInstance<IBankAdapter>()));

            For<IBankRequestClient>().Use<BankRequestClient>();

            For<HttpClient>().Use(container => ConfigureHttpClient());
        }

        private static ILogger CreateLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("Logs/PaymentsApiLogs.txt")
                .CreateLogger();

            return Log.Logger;
        }

        private static HttpClient ConfigureHttpClient()
        {

            var bankResponses = new List<BankResponse>
            {
                new BankResponse(Guid.NewGuid().ToString(), true),
                new BankResponse(Guid.NewGuid().ToString(), false)
            };

            var mockHttp = new MockHttpMessageHandler();

            var randomBankResponse = JsonConvert.SerializeObject(bankResponses.FirstOrDefault());

            mockHttp.When("http://dummy-bank-url/api/validate-method")
                .Respond("application/json", randomBankResponse);

            return new HttpClient(mockHttp);
        }

    }
}
