using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Services.Interfaces {
    public interface IGameLogicService {
        int GetNumberOfCpuPlayers();
        int GetRandomNumberOfTickets();
        GameResult GenerateResult(IEnumerable<Player> players);
    }
}
