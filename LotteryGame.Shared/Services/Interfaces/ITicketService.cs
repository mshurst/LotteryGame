namespace LotteryGame.Shared.Services.Interfaces;

public interface ITicketService {
    List<int> GetAllTicketsForGame(IEnumerable<Player> players);
}