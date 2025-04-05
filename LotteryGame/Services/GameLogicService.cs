using LotteryGame.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LotteryGame.Services {
    public class GameLogicService : IGameLogicService {
        private readonly IRandomGenerator randomNumberGenerator;
        private readonly LotteryGameSettings lotteryGameSettings;
        private readonly ILogger<GameLogicService> logger;
        private readonly ITicketService ticketService;

        public GameLogicService(IRandomGenerator randomNumberGenerator, IOptions<LotteryGameSettings> settings, ILogger<GameLogicService> logger, ITicketService ticketService) {
            this.randomNumberGenerator = randomNumberGenerator;
            this.logger = logger;
            this.ticketService = ticketService;
            this.lotteryGameSettings = settings.Value;
        }

        public GameResult GenerateResult(IEnumerable<Player> players) {
            var result = new GameResult();

            var prizes = lotteryGameSettings.PrizeSettings;

            var totalTickets = ticketService.GetAllTicketsForGame(players);

            var totalPrizePot = totalTickets.Count() * lotteryGameSettings.CostPerTicket;

            foreach (var prize in prizes) {
                var numberOfWinners = prize.NumberOfWinningTickets 
                                      ?? (int)Math.Ceiling((double)totalTickets.Count() * prize.PercentageOfWinningTickets.Value);

                var winningTickets = totalTickets.Take(numberOfWinners).ToList();

                totalTickets.RemoveRange(0, numberOfWinners);

                result.Winners.Add(prize.Name, winningTickets);
                result.Prizes.Add(prize.Name, totalPrizePot * prize.PrizeShare);
            }

            result.HouseShare = totalPrizePot - result.Prizes.Sum(x => x.Value);

            return result;
        }

        public int GetNumberOfCpuPlayers() {
            if (lotteryGameSettings.MinNumberOfPlayers > lotteryGameSettings.MaxNumberOfPlayers) {
                throw new ApplicationException("MinNumberOfPlayers cannot be greater than MaxNumberOfPlayers");
            }

            return randomNumberGenerator.GetRandomNumber(lotteryGameSettings.MinNumberOfPlayers, lotteryGameSettings.MaxNumberOfPlayers);
        }

        public int GetRandomNumberOfTickets() {
            if (lotteryGameSettings.MinNumberOfTicketsPerPlayer > lotteryGameSettings.MaxNumberOfTicketsPerPlayer) {
                throw new ApplicationException("MinNumberOfTicketsPerPlayer cannot be greater than MaxNumberOfTicketsPerPlayer");
            }
            
            return randomNumberGenerator.GetRandomNumber(lotteryGameSettings.MinNumberOfTicketsPerPlayer, lotteryGameSettings.MaxNumberOfTicketsPerPlayer);
        }
    }
}
