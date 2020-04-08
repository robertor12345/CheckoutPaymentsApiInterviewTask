using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.Gateway.Payments.Contracts;
using Checkout.Gateway.PaymentsApi.Adapters;

namespace Checkout.Gateway.Payments.Api.Adapters
{
    public class BankAdapter : IBankAdapter
    {
        private readonly  List<BankResponse> BankResponses;

        public BankAdapter()
        {
            BankResponses.Add(new BankResponse(new Guid().ToString(), true ));
            BankResponses.Add(new BankResponse(new Guid().ToString(), false ));
            BankResponses.Add(new BankResponse(new Guid().ToString(), true));
            BankResponses.Add(new BankResponse(new Guid().ToString(), false));
            BankResponses.Add(new BankResponse(new Guid().ToString(), true));
            BankResponses.Add(new BankResponse(new Guid().ToString(), false));
            BankResponses.Add(new BankResponse(new Guid().ToString(), true));
            BankResponses.Add(new BankResponse(new Guid().ToString(), false));
            BankResponses.Add(new BankResponse(new Guid().ToString(), true));
            BankResponses.Add(new BankResponse(new Guid().ToString(), false));
            BankResponses.Add(new BankResponse(new Guid().ToString(), true));
            BankResponses.Add(new BankResponse(new Guid().ToString(), false));

        }

        public Task<BankResponse> CallApi(PaymentMethodDetails paymentMethod, string currency, double amount)
        {
            var rnd = new Random();
            int r = rnd.Next(BankResponses.Count);

            return Task.FromResult(BankResponses[r]);
        }
    }
}