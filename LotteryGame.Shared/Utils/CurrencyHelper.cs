namespace LotteryGame.Shared.Utils;

public class CurrencyHelper {
    public string CurrencySymbol { get; set; }

    public CurrencyHelper(string currencySymbol) {
        CurrencySymbol = currencySymbol;
    }

    public string FormatCurrencyAsString(int amount) {
        return $"{CurrencySymbol}{(amount / 100.0):0.00}";
    }
}