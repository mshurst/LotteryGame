namespace LotteryGame.Services.Interfaces;

public interface ITicketService {
    List<int> GetAllTicketsForGame(IEnumerable<Player> players);
}