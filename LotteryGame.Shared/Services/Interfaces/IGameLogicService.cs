namespace LotteryGame.Shared.Services.Interfaces {
    public interface IGameLogicService {
        int GetNumberOfCpuPlayers();
        int GetRandomNumberOfTickets();
        GameResult GenerateResult(int numberOfTicketsForPlayer);
    }
}
