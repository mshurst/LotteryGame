using FluentAssertions;
using LotteryGame.Shared.Services;

namespace LotteryGame.UnitTests;

public class RandomGeneratorTests {
    [Fact]
    public void GetRandomNumber_WithRange_IncludesBothMinAndMax() {
        var service = new RandomGenerator();

        var results = new List<int>();
        for (int i = 0; i < 10000; i++) {
            results.Add(service.GetRandomNumber(1, 10));
        }

        results.Any(x => x == 10).Should().BeTrue();
        results.Any(x => x == 1).Should().BeTrue();
        results.Any(x => x == 11).Should().BeFalse();
        results.Any(x => x == 0).Should().BeFalse();
    }
}