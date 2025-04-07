using FluentAssertions;
using LotteryGame.Shared;
using LotteryGame.Shared.Services;
using Microsoft.Extensions.Options;

namespace LotteryGame.UnitTests;

public class TicketServiceTests {
    private TicketService service;

    public TicketServiceTests() {
        var options = Options.Create(new LotteryGameSettings() {
            CostPerTicket = 100,
            PlayerStartingBalance = 1000,
        });

        service = new TicketService(options);
    }

    [Fact]
    public void GetAllTicketsForGame_WithSinglePlayer_ReturnsCorrectList() {
        var result = service.GetAllTicketsForGame(new List<Player>()
        {
            new Player(1, 10)
        });

        result.Should().BeEquivalentTo(new List<int>() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
    }

    [Fact]
    public void GetAllTicketsForGame_WithMultiplePlayers_ReturnsCorrectList() {
        var players = new List<Player>() {
            new Player(1, 2),
            new Player(2, 3),
        };

        var result = service.GetAllTicketsForGame(players);

        result.Should().BeEquivalentTo(new List<int> { 1, 1, 2, 2, 2 });
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void ValidateCanAffordTickets_CanAfford_ReturnsTrue(int numberOfTickets) {
        var options = Options.Create(new LotteryGameSettings() {
            CostPerTicket = 100,
            PlayerStartingBalance = 1000,
        });

        service = new TicketService(options);

        service.ValidateCanAffordTickets(numberOfTickets).Should().BeTrue();
    }

    [Theory]
    [InlineData(11)]
    [InlineData(12)]
    [InlineData(int.MaxValue)]
    public void ValidateCanAffordTickets_CannotAfford_ReturnsFalse(int numberOfTickets) {
        var options = Options.Create(new LotteryGameSettings() {
            CostPerTicket = 100,
            PlayerStartingBalance = 1000,
        });

        service = new TicketService(options);

        service.ValidateCanAffordTickets(numberOfTickets).Should().BeFalse();
    }

    [Theory]
    [InlineData(1000, 100, 10)]
    [InlineData(1000, 101, 9)]
    [InlineData(1000, 99, 10)]
    [InlineData(123, 7, 17)]

    public void AllocateMaxNumberOfTickets_ReturnsCorrectValue(int balance, int costPerTicket, int expectedTickets) {
        var options = Options.Create(new LotteryGameSettings() {
            CostPerTicket = costPerTicket,
            PlayerStartingBalance = balance,
        });

        service = new TicketService(options);
        var result = service.AllocateMaxNumberOfTickets(balance, costPerTicket);
        result.Should().Be(expectedTickets);
    }
}