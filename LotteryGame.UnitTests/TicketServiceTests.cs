using FluentAssertions;
using LotteryGame.Services;

namespace LotteryGame.UnitTests;

public class TicketServiceTests {
    private TicketService service = new TicketService();

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
}