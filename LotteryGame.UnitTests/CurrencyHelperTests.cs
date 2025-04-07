using FluentAssertions;
using LotteryGame.Shared.Utils;

namespace LotteryGame.UnitTests;

public class CurrencyHelperTests {
    [Theory]
    [InlineData(1000, "10.00", "$")]
    [InlineData(1, "0.01", "£")]
    [InlineData(1234, "12.34", "€")]
    public void FormatCurrencyAsString_ReturnsExpectedValue(int input, string expected, string currencySymbol) {
        var service = new CurrencyHelper(currencySymbol);
        var result = service.FormatCurrencyAsString(input);
        result.Should().StartWith(currencySymbol);
        result.Should().Be($"{currencySymbol}{expected}");
    }
}