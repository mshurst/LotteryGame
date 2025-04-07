namespace LotteryGame.Shared.Services.Interfaces;

public interface ITicketService {
    List<int> GetAllTicketsForGame(IEnumerable<Player> players);
    bool ValidateCanAffordTickets(int numberOfTicketsToBuy);
    int AllocateMaxNumberOfTickets(int balance, int costPerTicket);
}