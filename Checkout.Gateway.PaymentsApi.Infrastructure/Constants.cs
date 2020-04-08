using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Checkout.Gateway.PaymentsApi.Infrastructure
{
    public static class Constants
    {
        public const string SqlConnectionString =
            "Server = (LocalDb)\\MSSQLLocalDB;Database=PaymentsDatabase;Trusted_Connection=True;";

        public static readonly IList<string> Currencies = new ReadOnlyCollection<string>
        (new List<string>
        {
            "GBP",
            "EUR",
            "USD",
            "AUD",
            "JPY",
            "CAD"
        });
    }
}