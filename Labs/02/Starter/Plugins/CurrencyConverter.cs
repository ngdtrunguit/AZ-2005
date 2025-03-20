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
            { "EUR-USD", 1.18m }
        };

        return exchangeRates.TryGetValue($"{fromCurrency}-{toCurrency}", out var rate) ? rate : throw new Exception("Exchange rate not found");
    }
}