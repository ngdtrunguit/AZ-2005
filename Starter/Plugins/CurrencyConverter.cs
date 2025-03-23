using System;
using System.ComponentModel;
using Microsoft.SemanticKernel;

class CurrencyConverter
{
    [KernelFunction("convert_currency")]
    [Description("Converts an amount from one currency to another, for example USD to EUR")]
    public static decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
    {
        decimal exchangeRate = GetExchangeRate(fromCurrency, toCurrency);
        return amount * exchangeRate;
    }

    private static decimal GetExchangeRate(string fromCurrency, string toCurrency)
    {
        var exchangeRates = new Dictionary<string, decimal>
        {
            { "USD-HKD", 7.77m },
            { "HKD-USD", 1 / 7.77m },
            { "USD-EUR", 0.85m },
            { "EUR-USD", 1.18m },
            { "USD-GBP", 0.75m },
            { "GBP-USD", 1.33m },
            { "USD-JPY", 110.50m },
            { "JPY-USD", 1 / 110.50m },
            { "USD-CNY", 6.45m },
            { "CNY-USD", 1 / 6.45m },
            { "USD-VND", 25000m},
            { "VND-USD", 1 / 25000m}
        };

        return exchangeRates.TryGetValue($"{fromCurrency}-{toCurrency}", out var rate) ? rate : throw new Exception("Exchange rate not found");
    }
}